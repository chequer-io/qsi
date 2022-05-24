using System;
using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.PostgreSql.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.PostgreSql.Internal.PostgreSqlParserInternal;

namespace Qsi.PostgreSql.Tree.Visitors;

internal static class TableVisitor
{
    public static QsiTableNode VisitSelectStatement(SelectStatementContext context)
    {
        switch (context.children[0])
        {
            case QueryExpressionParensContext queryExpressionParens:
                return VisitQueryExpressionParens(queryExpressionParens);
            
            case QueryExpressionContext queryExpression:
                return VisitQueryExpression(queryExpression);
            
            default:
                throw TreeHelper.NotSupportedTree(context.children[0]);
        }
    }

    public static QsiTableNode VisitQueryExpressionParens(in QueryExpressionParensContext context)
    {
        var queryExpression = UnwrapQueryExpressionParens(context);
        var node = VisitQueryExpression(queryExpression);

        PostgreSqlTree.Span[node] = new Range(context.Start.StartIndex, context.Stop.StopIndex + 1);

        return node;
    }

    public static QueryExpressionContext UnwrapQueryExpressionParens(QueryExpressionParensContext context)
    {
        var nested = context;

        while (nested.queryExpressionParens() != null)
        {
            nested = context.queryExpressionParens();
        }

        return nested.queryExpression();
    }

    public static QsiTableNode VisitQueryExpression(in QueryExpressionContext context)
    {
        var nowith = context.queryExpressionNoWith();
        var source = nowith.queryExpressionBody() != null ?
            VisitQueryExpressionBody(nowith.queryExpressionBody()) :
            VisitQueryExpressionParens(nowith.queryExpressionParens());

        var with = context.withClause();
        var orderByClause = nowith.orderByClause();
        var limitClause = nowith.limitClause();
        var lockingClause = nowith.forClause();

        if (with == null &&
            orderByClause == null &&
            limitClause == null &&
            lockingClause == null)
        {
            return source;
        }

        var node = new PostgreSqlDerivedTableNode();
        
        // Visit WITH clause
        if (with != null)
        {
            node.Directives.SetValue(VisitWithClause(with));
        }
        
        node.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
        
        // Set source
        node.Source.SetValue(source);

        if (orderByClause != null)
        {
            node.Order.SetValue(ExpressionVisitor.VisitOrderByClause(orderByClause));
        }

        if (limitClause != null)
        {
            node.Limit.SetValue(ExpressionVisitor.VisitLimitClause(limitClause));
        }

        if (lockingClause != null)
        {
            node.Locking.SetValue(VisitLockingClause(lockingClause));
        }
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }
    
    public static QsiTableDirectivesNode VisitWithClause(in WithClauseContext context)
    {
        var node = new QsiTableDirectivesNode
        {
            IsRecursive = context.HasToken(RECURSIVE)
        };
        
        node.Tables.AddRange(context.commonTableExpression().Select(VisitCommonTableExpression));
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiDerivedTableNode VisitCommonTableExpression(CommonTableExpressionContext context)
    {
        var node = new QsiDerivedTableNode();
        
        var columnIdentifierList = context.columnIdentifierList();
        node.Columns.SetValue(columnIdentifierList == null ?
            TreeHelper.CreateAllColumnsDeclaration() :
            CreateSequentialColumns(columnIdentifierList));

        var subqueryName = context.subqueryName().columnIdentifier();
        node.Alias.SetValue(new QsiAliasNode
        {
            Name = IdentifierVisitor.VisitIdentifier(subqueryName)
        });
        
        node.Source.SetValue(VisitCommonTableExpressionStatements(context.commonTableExpressionStatements()));
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiColumnsDeclarationNode CreateSequentialColumns(ColumnIdentifierListContext context)
    {
        var node = new QsiColumnsDeclarationNode();
        
        node.Columns.AddRange(context.columnIdentifier().Select(VisitSequentialColumn));
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiSequentialColumnNode VisitSequentialColumn(ColumnIdentifierContext context)
    {
        var identifier = IdentifierVisitor.VisitIdentifier(context);
        var alias = new QsiAliasNode { Name = identifier };

        var node = new QsiSequentialColumnNode
        {
            ColumnType = QsiSequentialColumnType.Overwrite,
            Alias = { Value = alias }
        };

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiTableNode VisitCommonTableExpressionStatements(CommonTableExpressionStatementsContext context)
    {
        return context.children[0] switch
        {
            SelectStatementContext select => VisitSelectStatement(select),
            // NOTE: Although PG supports other DML queries such as UPDATE, INSERT and DELETE, we don't support them.
            _ => throw TreeHelper.NotSupportedTree(context.children[0])
        };
    }

    public static PostgreSqlLockingNode VisitLockingClause(ForClauseContext context)
    {
        var lockStrengthOption = context.lockStrengthOption();
        var tableLockType = GetTableLockType(lockStrengthOption);
        
        var rowLockType = GetRowLockType(context);

        var tables = context
            .tableList()
            .qualifiedIdentifierList()
            .qualifiedIdentifier()
            .Select(IdentifierVisitor.VisitQualifiedIdentifier)
            .ToArray();

        var node = new PostgreSqlLockingNode
        {
            TableLockType = tableLockType,
            RowLockType = rowLockType,
            Tables = tables
        };
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    private static PostgreSqlTableLockType GetTableLockType(LockStrengthOptionContext context)
    {
        if (context.UPDATE() != null)
        {
            return context.NO() == null || context.KEY() == null ?
                PostgreSqlTableLockType.Update :
                PostgreSqlTableLockType.NoKeyUpdate;
        }

        if (context.SHARE() != null)
        {
            return context.KEY() == null ?
                PostgreSqlTableLockType.Share :
                PostgreSqlTableLockType.KeyShare;
        }

        throw TreeHelper.NotSupportedTree(context);
    }

    private static PostgreSqlRowLockType? GetRowLockType(ForClauseContext context)
    {
        if (context.NOWAIT() != null)
        {
            return PostgreSqlRowLockType.NoWait;
        }

        if (context.SKIP_P() != null && context.LOCKED() != null)
        {
            return PostgreSqlRowLockType.SkipLocked;
        }

        return null;
    }

    public static QsiTableNode VisitQueryExpressionBody(in QueryExpressionBodyContext context)
    {
        var primary = context.queryPrimary() != null ?
            VisitQueryPrimary(context.queryPrimary()) :
            VisitQueryExpressionParens(context.queryExpressionParens());

        var expressionSetNodes = context.queryExpressionSet()
            ?.Select(c => VisitQueryExpressionSet(c))
            .ToArray();

        if (expressionSetNodes == null || expressionSetNodes.Length == 0)
        {
            return primary;
        }

        var node = new QsiCompositeTableNode();
        
        node.Sources.Add(primary);
        node.Sources.AddRange(expressionSetNodes);
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }
    
    public static QsiTableNode VisitQueryExpressionSet(in QueryExpressionSetContext context)
    {
        return context.queryPrimary() != null ?
            VisitQueryPrimary(context.queryPrimary()) :
            VisitQueryExpressionParens(context.queryExpressionParens());
    }

    public static QsiTableNode VisitQueryPrimary(in QueryPrimaryContext context)
    {
        return context switch
        {
            SelectQueryPrimaryContext select => VisitSelectQueryPrimary(select),
            ValuesQueryPrimaryContext values => VisitValuesQueryPrimary(values),
            TableQueryPrimaryContext table => VisitTableQueryPrimary(table),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    public static QsiTableNode VisitSelectQueryPrimary(SelectQueryPrimaryContext context)
    {
        var node = new PostgreSqlDerivedTableNode();

        var option = context.selectOption();
        var itemList = context.selectItemList();
        var fromClause = context.fromClause();
        var whereClause = context.whereClause();
        var groupByClause = context.groupByClause();
        var havingClause = context.havingClause();

        if (option != null)
        {
            node.SelectOptions.SetValue(VisitSelectOption(option));
        }

        if (itemList != null)
        {
            node.Columns.SetValue(VisitSelectItemList(itemList));
        }

        if (fromClause != null)
        {
            var source = VisitFromClause(fromClause);

            if (source != null)
            {
                node.Source.SetValue(source);
            }
        }

        if (whereClause != null)
        {
            node.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));
        }

        if (groupByClause != null)
        {

            node.Grouping.SetValue(ExpressionVisitor.VisitGroupByClause(groupByClause));
            
            if (havingClause != null)
            {
                node.Grouping.Value.Having.Value = ExpressionVisitor.VisitExpression(havingClause.expression());
            }
        }

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static PostgreSqlSelectOptionNode VisitSelectOption(SelectOptionContext context)
    {
        var node = new PostgreSqlSelectOptionNode();

        if (context.ALL() != null)
        {
            node.Option = PostgreSqlSelectOption.All;
        }
        
        if (context.DISTINCT() != null)
        {
            node.Option = PostgreSqlSelectOption.Distinct;

            if (context.expressionList() != null)
            {
                var expressions = context.expressionList().expression();
                var expressionNodes = expressions.Select(ExpressionVisitor.VisitExpression);
                node.DistinctExpressionList.AddRange(expressionNodes);
            }
        }
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiColumnsDeclarationNode VisitSelectItemList(SelectItemListContext context)
    {
        var node = new QsiColumnsDeclarationNode();

        foreach (var child in context.children)
        {
            switch (child)
            {
                case ITerminalNode { Symbol: {Type: STAR} }:
                    node.Columns.Add(new QsiAllColumnNode());
                    break;
                
                case SelectItemContext selectItem:
                    node.Columns.Add(VisitSelectItem(selectItem));
                    break;
            }
        }

        // TODO: Ask whether I should put a span here.
        PostgreSqlTree.PutContextSpan(node, context);
        return node;
    }

    public static QsiColumnNode VisitSelectItem(SelectItemContext context)
    {
        if (context.STAR() != null)
        {
            var allColumnNode = new QsiAllColumnNode();
            
            PostgreSqlTree.PutContextSpan(allColumnNode, context);

            return allColumnNode;
        }

        var columnNode = VisitColumnExpression(context.expression());
        var aliasClause = context.aliasClause();

        if (aliasClause == null)
        {
            return columnNode;
        }
        
        if (columnNode is not QsiDerivedColumnNode derivedColumnNode)
        {
            derivedColumnNode = new QsiDerivedColumnNode
            {
                Column = { Value = columnNode }
            };
        }

        derivedColumnNode.Alias.Value = VisitAliasClause(aliasClause);
            
        PostgreSqlTree.PutContextSpan(derivedColumnNode, context);

        return derivedColumnNode;
    }
    
    public static QsiColumnNode VisitColumnExpression(ExpressionContext context)
    {
        var expressionNode = ExpressionVisitor.VisitExpression(context);

        if (expressionNode is QsiColumnExpressionNode columnExpression)
        {
            return columnExpression.Column.Value;
        }

        var node = new QsiDerivedColumnNode();
        node.Expression.SetValue(expressionNode);
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiTableNode VisitValuesQueryPrimary(ValuesQueryPrimaryContext context)
    {
        var node = new QsiInlineDerivedTableNode();
        node.Rows.AddRange(context.valueList().valueItem().Select(VisitRowValueItem));
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiTableNode VisitFromClause(FromClauseContext context)
    {
        var fromItems = context.fromItemList().fromItem()
            .Select(VisitFromItem)
            .ToArray();

        if (fromItems.Length == 1)
        {
            return fromItems[0];
        }

        var anchor = fromItems[0];

        for (int i = 1; i < fromItems.Length; ++i)
        {
            var join = new QsiJoinedTableNode
            {
                IsComma = true
            };
            
            join.Left.SetValue(anchor);
            join.Right.SetValue(fromItems[i]);

            var leftSpan = PostgreSqlTree.Span[join.Left.Value];
            var rightSpan = PostgreSqlTree.Span[join.Right.Value];

            PostgreSqlTree.Span[join] = new Range(leftSpan.Start, rightSpan.End);

            anchor = join;
        }

        return anchor;
    }

    public static QsiTableNode VisitFromItem(FromItemContext context)
    {
        var source =context.fromItemParens() != null 
            ? VisitFromItemParens(context.fromItemParens())
            : VisitFromItemPrimary(context.fromItemPrimary());

        foreach (var joinClause in context.joinClause())
        {
            source = VisitJoinClause(joinClause, source);
        }

        return source;
    }

    public static QsiTableNode VisitFromItemParens(FromItemParensContext context)
    {
        var nested = context;
        
        while (nested.fromItemParens() != null)
        {
            nested = context.fromItemParens();
        }
        
        var node = VisitFromItem(nested.fromItem());

        PostgreSqlTree.Span[node] = new Range(context.Start.StartIndex, context.Stop.StopIndex + 1);

        return node;
    }

    public static QsiTableNode VisitFromItemPrimary(FromItemPrimaryContext context)
    {
        if (context.tableFromItem() != null)
        {
            return VisitTableFromItem(context.tableFromItem());
        }

        if (context.functionFromItem() != null)
        {
            return VisitFunctionFromItem(context.functionFromItem());
        }

        if (context.subqueryFromItem() != null)
        {
            return VisitSubqueryFromItem(context.subqueryFromItem());
        }

        throw TreeHelper.NotSupportedTree(context.children[0]);
    }

    public static QsiTableNode VisitTableFromItem(TableFromItemContext context)
    {
        var tableNode = VisitTableReference(context.tableName());

        if (context.aliasClause() == null)
        {
            return tableNode;
        }

        var aliasClause = context.aliasClause();

        var node = new QsiDerivedTableNode
        {
            Columns = { Value = TreeHelper.CreateAllColumnsDeclaration() },
            Source = { Value = tableNode },
            Alias = { Value = VisitAliasClause(aliasClause) }
        };

        var columnList = aliasClause.aliasClauseBody().columnIdentifierList();

        node.Columns.Value = columnList == null ?
            TreeHelper.CreateAllColumnsDeclaration() :
            CreateSequentialColumns(columnList);

        return node;
    }

    public static QsiTableNode VisitFunctionFromItem(FunctionFromItemContext context)
    {
        throw TreeHelper.NotSupportedFeature("Table function");
    }

    public static QsiTableNode VisitSubqueryFromItem(SubqueryFromItemContext context)
    {
        var subqueryNode = VisitQueryExpressionParens(context.queryExpressionParens());
        
        var aliasBody = context.aliasClause().aliasClauseBody();

        if (aliasBody == null)
        {
            return subqueryNode;
        }

        var node = new QsiDerivedTableNode();
        var columnList = aliasBody.columnIdentifierList();
        
        node.Source.SetValue(subqueryNode);
        node.Alias.SetValue(VisitAliasClause(context.aliasClause()));
        
        node.Columns.SetValue(columnList == null ?
            TreeHelper.CreateAllColumnsDeclaration() :
            CreateSequentialColumns(columnList));
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiTableNode VisitJoinClause(JoinClauseContext context, QsiTableNode left)
    {
        var node = new QsiJoinedTableNode();
        var join = context.join();

        node.Left.SetValue(left);
        node.IsNatural = join.NATURAL() == null;
        node.JoinType = join.joinType()?.GetInputText()
                        ?? join.CROSS()?.GetText()
                        ?? "OUTER";
        node.Right.SetValue(VisitFromItemPrimary(join.fromItemPrimary()));
        
        if (context.expression() != null)
        {
            node.PivotExpression.SetValue(ExpressionVisitor.VisitExpression(context.expression()));
        }
        else if (context.columnIdentifierList() != null)
        {
            node.PivotColumns.SetValue(VisitIdentifierList(context.columnIdentifierList()));
        }
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiColumnsDeclarationNode VisitIdentifierList(ColumnIdentifierListContext context)
    {
        var node = new QsiColumnsDeclarationNode();
        
        var identifiers = context.columnIdentifier().Select(IdentifierVisitor.VisitIdentifier);
        node.Columns.AddRange(identifiers.Select(i => 
            new QsiColumnReferenceNode {Name = new QsiQualifiedIdentifier(i)}));
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiRowValueExpressionNode VisitRowValueItem(ValueItemContext context)
    {
        var valueColumns = context.valueColumnList().valueColumn();
        var values = valueColumns
            .Select(v => v.expression() != null ?
                ExpressionVisitor.VisitExpression(v.expression()) :
                TreeHelper.CreateDefaultLiteral());
        
        var node = new QsiRowValueExpressionNode();
        node.ColumnValues.AddRange(values);
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiTableNode VisitTableQueryPrimary(TableQueryPrimaryContext context)
    {
        var tableName = context.tableName().qualifiedIdentifier();

        var node = new PostgreSqlExplicitTableReferenceNode
        {
            Identifier = IdentifierVisitor.VisitQualifiedIdentifier(tableName)
        };
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }
    
    public static QsiTableReferenceNode VisitTableReference(TableNameContext context)
    {
        var node = new QsiTableReferenceNode
        {
            Identifier = IdentifierVisitor.VisitQualifiedIdentifier(context.qualifiedIdentifier())
        };

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiAliasNode VisitAliasClause(AliasClauseContext context)
    {
        var aliasIdentifier = context.aliasClauseBody().columnIdentifier();
        var aliasName = IdentifierVisitor.VisitIdentifier(aliasIdentifier);

        var node = new QsiAliasNode { Name = aliasName };
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }
}

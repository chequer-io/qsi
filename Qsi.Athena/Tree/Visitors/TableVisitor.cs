using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Athena.Internal;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors;

using static SqlBaseParser;

internal static class TableVisitor
{
    public static QsiTableNode VisitQuery(QueryContext context)
    {
        var with = context.with();
        var queryNoWith = context.queryNoWith();

        var queryNoWithNode = VisitQueryNoWith(queryNoWith);

        if (with is null)
            return queryNoWithNode;

        if (queryNoWithNode is not QsiDerivedTableNode derivedTable)
        {
            derivedTable = new QsiDerivedTableNode
            {
                Source = { Value = queryNoWithNode },
                Columns = { Value = TreeHelper.CreateAllVisibleColumnsDeclaration() }
            };

            queryNoWithNode = derivedTable;
        }
        else
        {
            AthenaTree.PutContextSpan(queryNoWithNode, context);
        }

        derivedTable.Directives.Value = VisitWith(with);

        return queryNoWithNode;
    }

    private static QsiTableNode VisitQueryNoWith(QueryNoWithContext context)
    {
        var queryTerm = context.queryTerm();
        var orderBy = context.orderBy();

        var limitOffsetTerm = context.limitOffsetTerm();

        var queryNode = VisitQueryTerm(queryTerm);

        if (queryNode is QsiDerivedTableNode node)
        {
            AthenaTree.PutContextSpan(node, context);
        }
        else
        {
            var derivedTableNode = AthenaTree.CreateWithSpan<QsiDerivedTableNode>(context);
            derivedTableNode.Columns.Value = TreeHelper.CreateAllVisibleColumnsDeclaration();
            derivedTableNode.Source.Value = queryNode;

            node = derivedTableNode;
        }

        if (orderBy is not null)
        {
            var orderByNode = ExpressionVisitor.VisitOrderBy(orderBy);
            node.Order.Value = orderByNode;
        }

        if (limitOffsetTerm is not null)
        {
            var limitOffsetNode = ExpressionVisitor.VisitLimitOffsetTerm(limitOffsetTerm);
            node.Limit.Value = limitOffsetNode;
        }

        return node;
    }

    private static QsiTableNode VisitQueryTerm(QueryTermContext context)
    {
        return context switch
        {
            QueryTermDefaultContext queryTermDefault => VisitQueryTermDefault(queryTermDefault),
            SetOperationContext setOperation => VisitSetOperation(setOperation),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiTableNode VisitQueryTermDefault(QueryTermDefaultContext context)
    {
        return VisitQueryPrimary(context.queryPrimary());
    }

    private static QsiTableNode VisitSetOperation(SetOperationContext context)
    {
        var left = context.left;
        var right = context.right;

        var @operator = context.@operator;
        var setQuantifier = context.setQuantifier();

        var operatorText = @operator.Text;

        var leftNode = VisitQueryTerm(left);
        var rightNode = VisitQueryTerm(right);

        var node = AthenaTree.CreateWithSpan<QsiCompositeTableNode>(context);

        if (setQuantifier is null)
        {
            node.CompositeType = operatorText;
        }
        else
        {
            var setQuantifierText = setQuantifier.GetText();
            node.CompositeType = $"{operatorText} {setQuantifierText}";
        }

        node.Sources.Add(leftNode);
        node.Sources.Add(rightNode);

        return node;
    }

    private static QsiTableNode VisitQueryPrimary(QueryPrimaryContext context)
    {
        return context switch
        {
            QueryPrimaryDefaultContext queryPrimaryDefault => VisitQueryPrimaryDefault(queryPrimaryDefault),
            TableContext table => VisitTable(table),
            InlineTableContext inlineTable => VisitInlineTable(inlineTable),
            SubqueryContext subquery => VisitSubquery(subquery),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiTableNode VisitQueryPrimaryDefault(QueryPrimaryDefaultContext context)
    {
        return VisitQuerySpecification(context.querySpecification());
    }

    private static QsiTableNode VisitTable(TableContext context)
    {
        var qualifiedName = context.qualifiedName();
        var qualifiedNameNode = VisitQualifiedName(qualifiedName);

        var node = AthenaTree.CreateWithSpan<QsiDerivedTableNode>(context);

        node.Columns.Value = TreeHelper.CreateAllVisibleColumnsDeclaration();
        node.Source.Value = qualifiedNameNode;

        return node;
    }

    private static QsiTableNode VisitInlineTable(InlineTableContext context)
    {
        ExpressionContext[] expressions = context.expression();

        var node = AthenaTree.CreateWithSpan<AthenaValuesTableNode>(context);

        foreach (var expression in expressions)
        {
            var rowNode = AthenaTree.CreateWithSpan<QsiRowValueExpressionNode>(expression);

            var expressionNode = ExpressionVisitor.VisitExpression(expression);

            if (expressionNode is QsiMultipleExpressionNode multipleExpr)
                foreach (var element in multipleExpr.Elements)
                    rowNode.ColumnValues.Add(element);
            else
                rowNode.ColumnValues.Add(expressionNode);

            node.Rows.Add(rowNode);
        }

        return node;
    }

    private static QsiTableNode VisitSubquery(SubqueryContext context)
    {
        var queryNoWith = context.queryNoWith();
        var node = VisitQueryNoWith(queryNoWith);

        AthenaTree.PutContextSpan(node, context);

        return node;
    }

    private static QsiTableNode VisitQuerySpecification(QuerySpecificationContext context)
    {
        var setQuantifier = context.setQuantifier();
        var fromTerm = context.fromTerm();
        var whereTerm = context.whereTerm();
        var groupByHavingTerm = context.groupByHavingTerm();

        var node = AthenaTree.CreateWithSpan<AthenaDerivedTableNode>(context);

        if (setQuantifier is not null)
            node.SetQuantifier = ExpressionVisitor.VisitSetQuantifier(setQuantifier);

        var columnsNode = new QsiColumnsDeclarationNode();
        var columnIndex = 0;

        foreach (var column in context.selectItem().Select(VisitSelectItem))
        {
            if (column is QsiDerivedColumnNode derivedColumn && !derivedColumn.Expression.IsEmpty)
                derivedColumn.InferredName = new QsiIdentifier($"_col{columnIndex}", false);

            columnsNode.Columns.Add(column);
            columnIndex++;
        }

        node.Columns.Value = columnsNode;

        if (fromTerm is not null)
        {
            var sourceNode = VisitFromTerm(fromTerm);
            node.Source.Value = sourceNode;
        }

        if (whereTerm is not null)
        {
            var whereNode = ExpressionVisitor.VisitWhereTerm(whereTerm);
            node.Where.Value = whereNode;
        }

        if (groupByHavingTerm is not null)
        {
            var groupByHavingTermNode = ExpressionVisitor.VisitGroupByHavingTerm(groupByHavingTerm);
            node.Grouping.Value = groupByHavingTermNode;
        }

        return node;
    }

    private static QsiColumnNode VisitSelectItem(SelectItemContext context)
    {
        switch (context)
        {
            case SelectSingleContext selectSingle:
                return VisitSelectSingle(selectSingle);

            case SelectAllContext selectAll:
                return VisitSelectAll(selectAll);

            default:
                throw TreeHelper.NotSupportedTree(context);
        }
    }

    private static QsiColumnNode VisitSelectSingle(SelectSingleContext context)
    {
        var expression = context.expression();
        var identifier = context.identifier();

        var expressionNode = ExpressionVisitor.VisitExpression(expression);

        QsiColumnNode node;

        if (expressionNode is QsiColumnExpressionNode columnExpressionNode)
        {
            node = columnExpressionNode.Column.Value;
        }
        else
        {
            var derivedColumnNode = new QsiDerivedColumnNode
            {
                Expression = { Value = expressionNode }
            };

            node = derivedColumnNode;
        }

        if (identifier is not null)
        {
            if (node is not QsiDerivedColumnNode derivedColumnNode)
            {
                derivedColumnNode = new QsiDerivedColumnNode
                {
                    Column = { Value = node }
                };

                node = derivedColumnNode;
            }

            derivedColumnNode.Alias.Value = new QsiAliasNode
            {
                Name = identifier.qi
            };
        }

        return node;
    }

    private static QsiColumnNode VisitSelectAll(SelectAllContext context)
    {
        var qualifiedName = context.qualifiedName();

        var node = AthenaTree.CreateWithSpan<QsiAllColumnNode>(context);

        if (qualifiedName is not null)
            node.Path = qualifiedName.qqi;

        return node;
    }

    private static QsiTableNode VisitFromTerm(FromTermContext context)
    {
        RelationContext[] relations = context.relation();

        var node = VisitRelation(relations[0]);

        for (var i = 1; i < relations.Length; i++)
        {
            var relation = relations[i];
            var relationNode = VisitRelation(relation);

            var nodeSpan = AthenaTree.Span[node];
            var relationSpan = AthenaTree.Span[relationNode];

            var joinedTable = new QsiJoinedTableNode
            {
                IsComma = true,
                Left = { Value = node },
                Right = { Value = relationNode }
            };

            AthenaTree.Span[joinedTable] = new Range(nodeSpan.Start, relationSpan.End);
            node = joinedTable;
        }

        return node;
    }

    public static QsiTableNode VisitRelation(RelationContext context)
    {
        return context switch
        {
            JoinRelationContext joinRelation => VisitJoinRelation(joinRelation),
            RelationDefaultContext relationDefault => VisitRelationDefault(relationDefault),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiTableNode VisitJoinRelation(JoinRelationContext context)
    {
        var left = context.left;

        var leftNode = VisitRelation(left);

        var node = AthenaTree.CreateWithSpan<QsiJoinedTableNode>(context);
        node.Left.Value = leftNode;

        if (context.HasToken(CROSS))
        {
            var right = context.right;
            var rightNode = VisitSampledRelation(right);

            node.JoinType = "CROSS JOIN";
            node.Right.Value = rightNode;

            return node;
        }

        var joinType = context.joinType();

        IEnumerable<IParseTree> joinTypeChildren = joinType.children ?? Enumerable.Empty<IParseTree>();
        IEnumerable<string> joinTypeChildrenText = joinTypeChildren.Select(c => c.GetText());

        var joinTypeText = string.Join(" ", joinTypeChildrenText);
        var hasJoinType = string.IsNullOrEmpty(joinTypeText);

        if (context.HasToken(NATURAL))
        {
            var right = context.right;
            var rightNode = VisitSampledRelation(right);

            node.IsNatural = true;
            node.JoinType = hasJoinType ? "NATURAL JOIN" : $"NATURAL {joinTypeText} JOIN";
            node.Right.Value = rightNode;

            return node;
        }

        var rightRelation = context.rightRelation;
        var rightRelationNode = VisitRelation(rightRelation);

        node.JoinType = hasJoinType ? "JOIN" : $"{joinTypeText} JOIN";
        node.Right.Value = rightRelationNode;

        return node;
    }

    private static QsiTableNode VisitRelationDefault(RelationDefaultContext context)
    {
        return VisitSampledRelation(context.sampledRelation());
    }

    private static QsiTableNode VisitSampledRelation(SampledRelationContext context)
    {
        // Ignored TABLESAMPLE syntax
        return VisitAliasedRelation(context.aliasedRelation());
    }

    private static QsiTableNode VisitAliasedRelation(AliasedRelationContext context)
    {
        var relationPrimary = context.relationPrimary();
        var identifier = context.identifier();
        var columnAliases = context.columnAliases();

        var relationPrimaryNode = VisitRelationPrimary(relationPrimary);

        if (identifier is null)
            return relationPrimaryNode;

        if (relationPrimaryNode is QsiDerivedTableNode node)
        {
            AthenaTree.PutContextSpan(node, context);
        }
        else
        {
            var derivedTableNode = AthenaTree.CreateWithSpan<QsiDerivedTableNode>(context);
            derivedTableNode.Source.Value = relationPrimaryNode;
            derivedTableNode.Columns.Value = TreeHelper.CreateAllVisibleColumnsDeclaration();

            node = derivedTableNode;
        }

        node.Alias.Value = new QsiAliasNode { Name = identifier.qi };

        if (columnAliases is null)
            return node;

        IdentifierContext[] columnAliasIdentifers = columnAliases.identifier();
        var columnAliasesNode = AthenaTree.CreateWithSpan<QsiColumnsDeclarationNode>(columnAliases);

        foreach (var columnAliasIdentifier in columnAliasIdentifers)
        {
            var sequentialNode = AthenaTree.CreateWithSpan<QsiSequentialColumnNode>(columnAliasIdentifier);
            sequentialNode.ColumnType = QsiSequentialColumnType.Overwrite;
            sequentialNode.Alias.Value = new QsiAliasNode { Name = columnAliasIdentifier.qi };

            columnAliasesNode.Columns.Add(sequentialNode);
        }

        node.Columns.Value = columnAliasesNode;

        return node;
    }

    private static QsiTableNode VisitRelationPrimary(RelationPrimaryContext context)
    {
        return context switch
        {
            TableNameContext tableName => VisitTableName(tableName),
            SubqueryRelationContext subqueryRelation => VisitSubqueryRelation(subqueryRelation),
            UnnestContext unnest => VisitUnnest(unnest),
            LateralContext lateral => VisitLateral(lateral),
            ParenthesizedRelationContext parenthesizedRelation => VisitParenthesizedRelation(parenthesizedRelation),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiTableNode VisitTableName(TableNameContext context)
    {
        return VisitQualifiedName(context.qualifiedName());
    }

    private static QsiTableNode VisitSubqueryRelation(SubqueryRelationContext context)
    {
        return VisitQuery(context.query());
    }

    // ReSharper disable once UnusedParameter.Local
    private static QsiTableNode VisitUnnest(UnnestContext context)
    {
        throw TreeHelper.NotSupportedFeature("Unnest table");
    }

    private static QsiTableNode VisitLateral(LateralContext context)
    {
        var node = AthenaTree.CreateWithSpan<AthenaLateralTableNode>(context);
        node.Source.Value = VisitQuery(context.query());

        return node;
    }

    private static QsiTableNode VisitParenthesizedRelation(ParenthesizedRelationContext context)
    {
        return VisitRelation(context.relation());
    }

    public static QsiTableReferenceNode VisitQualifiedName(QualifiedNameContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiTableReferenceNode>(context);
        node.Identifier = context.qqi;

        return node;
    }

    private static QsiTableDirectivesNode VisitWith(WithContext context)
    {
        NamedQueryContext[] namedQueries = context.namedQuery();

        IEnumerable<QsiDerivedTableNode> namedQueryNodes = namedQueries.Select(VisitNamedQuery);

        var node = AthenaTree.CreateWithSpan<QsiTableDirectivesNode>(context);
        node.Tables.AddRange(namedQueryNodes);

        return node;
    }

    private static QsiDerivedTableNode VisitNamedQuery(NamedQueryContext context)
    {
        var name = context.name;
        var columnAliases = context.columnAliases();
        var query = context.query();

        var nameIdentifier = name.qi;

        var aliasNode = new QsiAliasNode
        {
            Name = nameIdentifier
        };

        var queryNode = VisitQuery(query);

        var node = AthenaTree.CreateWithSpan<QsiDerivedTableNode>(context);
        node.Alias.Value = aliasNode;

        if (columnAliases is null)
        {
            node.Columns.Value = TreeHelper.CreateAllVisibleColumnsDeclaration();
        }
        else
        {
            IdentifierContext[] identifiers = columnAliases.identifier();

            IEnumerable<QsiColumnReferenceNode> columnReferenceNodes = identifiers.Select(identifier => new QsiColumnReferenceNode
            {
                Name = new QsiQualifiedIdentifier(identifier.qi)
            });

            var columnAliasesNode = new QsiColumnsDeclarationNode();
            columnAliasesNode.Columns.AddRange(columnReferenceNodes);

            node.Columns.Value = columnAliasesNode;
        }

        node.Source.Value = queryNode;

        return node;
    }

    private static QsiTableNode CreateSource(RelationContext[] relations)
    {
        var source = VisitRelation(relations[0]);

        if (relations.Length != 1)
            for (var i = 1; i < relations.Length; i++)
            {
                var leftContext = relations[i - 1];
                var rightContext = relations[i];

                var joinedTable = AthenaTree.CreateWithSpan<QsiJoinedTableNode>(
                    leftContext.Start,
                    rightContext.Stop
                );

                joinedTable.IsComma = true;
                joinedTable.Left.Value = source;
                joinedTable.Right.Value = VisitRelation(rightContext);

                source = joinedTable;
            }

        return source;
    }

    #region private extension methods
    private static QsiColumnNode ToColumn(this QsiExpressionNode node)
    {
        QsiColumnNode columnNode;

        if (node is QsiColumnExpressionNode derivedColumn)
        {
            columnNode = derivedColumn.Column.Value;
        }
        else
        {
            var derivedColumnNode = new QsiDerivedColumnNode();
            derivedColumnNode.Expression.Value = node;
            columnNode = derivedColumnNode;
        }

        return columnNode;
    }

    private static QsiDerivedColumnNode ToDerivedColumn(this QsiColumnNode node, ParserRuleContext context)
    {
        if (node is not QsiDerivedColumnNode derivedColumn)
        {
            derivedColumn = new QsiDerivedColumnNode();
            derivedColumn.Column.Value = node;
        }
        else
        {
            AthenaTree.PutContextSpan(node, context);
        }

        return derivedColumn;
    }

    private static QsiDerivedTableNode ToDerivedTable(this QsiTableNode node, ParserRuleContext context)
    {
        if (node is not QsiDerivedTableNode derivedTable)
        {
            derivedTable = new QsiDerivedTableNode();
            derivedTable.Source.Value = node;
            derivedTable.Columns.Value = TreeHelper.CreateAllVisibleColumnsDeclaration();
        }
        else
        {
            AthenaTree.PutContextSpan(node, context);
        }

        return derivedTable;
    }
    #endregion
}

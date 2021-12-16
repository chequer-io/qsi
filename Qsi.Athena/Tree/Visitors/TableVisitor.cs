using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Athena.Internal;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class TableVisitor
    {
        public static QsiTableNode VisitQuery(QueryContext context)
        {
            var table = VisitQueryNoWith(context.queryNoWith());
            var with = context.with();

            if (with is null)
                return table;

            var node = table.ToDerivedTable(context);
            node.Directives.Value = VisitWithClause(with);

            return node;
        }

        public static QsiTableNode VisitQueryNoWith(QueryNoWithContext context)
        {
            var table = VisitQueryTerm(context.queryTerm());

            if (table is AthenaValuesTableNode valuesInlineTable)
                return valuesInlineTable;

            var node = table.ToDerivedTable(context);

            if (context.HasToken(ORDER))
            {
                node.Order.Value = ExpressionVisitor.CreateMultipleOrderExpression(
                    context.sortItem(),
                    context.ORDER()
                );
            }

            IToken limitStartToken = null;
            IToken limitEndToken = null;
            QsiLimitExpressionNode limitNode = null;

            // OFFSET <offset> [ ROW | ROWS ]
            if (context.HasToken(OFFSET))
            {
                limitNode = new QsiLimitExpressionNode();

                limitStartToken = context.OFFSET().Symbol;
                limitEndToken = context.offsetRow ?? context.offset;

                limitNode.Offset.Value = TreeHelper.CreateLiteral(long.Parse(context.offset.Text));
            }

            // LIMIT { limitCount | ALL } <- skip when limit is all (unnessary when all)
            if (context.HasToken(LIMIT) && !context.HasToken(ALL))
            {
                limitNode ??= new QsiLimitExpressionNode();

                limitStartToken ??= context.LIMIT().Symbol;
                limitEndToken = context.limit;

                limitNode.Limit.Value = TreeHelper.CreateLiteral(context.limit.Text);
            }

            if (limitNode is not null)
            {
                node.Limit.Value = limitNode;
                AthenaTree.PutContextSpan(limitNode, limitStartToken, limitEndToken);
            }

            return node;
        }

        public static QsiTableNode VisitQueryTerm(QueryTermContext context)
        {
            switch (context)
            {
                case QueryTermDefaultContext queryTermDefault:
                    return VisitQueryPrimary(queryTermDefault.queryPrimary());

                case SetOperationContext setOperation:
                    return VisitSetOperation(setOperation);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableNode VisitQueryPrimary(QueryPrimaryContext context)
        {
            switch (context)
            {
                case QueryPrimaryDefaultContext queryPrimaryDefault:
                    return VisitQuerySpecification(queryPrimaryDefault.querySpecification());

                case TableContext table:
                    return VisitTable(table);

                case InlineTableContext inlineTable:
                    return VisitInlineTable(inlineTable);

                case SubqueryContext subquery:
                    return VisitQueryNoWith(subquery.queryNoWith());

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiDerivedTableNode VisitQuerySpecification(QuerySpecificationContext context)
        {
            var node = AthenaTree.CreateWithSpan<AthenaDerivedTableNode>(context);

            // Set Quantifier
            if (context.setQuantifier() is not null)
                node.SetQuantifier = VisitSetQuantifier(context.setQuantifier());

            // Columns
            var columnsNode = new QsiColumnsDeclarationNode();
            columnsNode.Columns.AddRange(context.selectItem().Select(VisitSelectItem));

            node.Columns.Value = columnsNode;

            // Source
            if (context.HasToken(FROM))
                node.Source.Value = CreateSource(context.relation());

            // Where Condition
            if (context.HasToken(WHERE))
            {
                var whereNode = AthenaTree.CreateWithSpan<QsiWhereExpressionNode>(
                    context.WHERE().Symbol,
                    context.where.Stop
                );

                whereNode.Expression.Value = ExpressionVisitor.VisitBooleanExpression(context.where);
                node.Where.Value = whereNode;
            }

            IToken groupingStartToken = null;
            IToken groupingEndToken = null;
            AthenaGroupingExpressionNode groupingNode = null;

            // Group By Clause
            if (context.HasToken(GROUP))
            {
                var groupByContext = context.groupBy();

                groupingStartToken = context.GROUP().Symbol;
                groupingEndToken = groupByContext.Stop;
                groupingNode = new AthenaGroupingExpressionNode();

                if (groupByContext.setQuantifier() is not null)
                    groupingNode.SetQuantifier = VisitSetQuantifier(groupByContext.setQuantifier());

                groupingNode.Items.AddRange(groupByContext.groupingElement().Select(ExpressionVisitor.VisitGroupingElement));
            }

            // Having Clause
            if (context.HasToken(HAVING))
            {
                groupingStartToken ??= context.HAVING().Symbol;
                groupingEndToken = context.having.Stop;
                groupingNode ??= new AthenaGroupingExpressionNode();

                groupingNode.Having.Value = ExpressionVisitor.VisitBooleanExpression(context.having);
            }

            if (groupingNode is not null)
            {
                AthenaTree.PutContextSpan(groupingNode, groupingStartToken, groupingEndToken);
                node.Grouping.Value = groupingNode;
            }

            return node;
        }

        public static QsiTableNode VisitTable(TableContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiDerivedTableNode>(context);

            node.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            node.Source.Value = VisitQualifiedName(context.qualifiedName());

            return node;
        }

        public static AthenaValuesTableNode VisitInlineTable(InlineTableContext context)
        {
            var node = AthenaTree.CreateWithSpan<AthenaValuesTableNode>(context);

            foreach (var expression in context.expression())
            {
                var rowNode = AthenaTree.CreateWithSpan<QsiRowValueExpressionNode>(expression);

                var expr = ExpressionVisitor.VisitExpression(expression);

                if (expr is QsiMultipleExpressionNode multipleExpr)
                {
                    foreach (var element in multipleExpr.Elements)
                        rowNode.ColumnValues.Add(element);
                }
                else
                {
                    rowNode.ColumnValues.Add(expr);
                }

                node.Rows.Add(rowNode);
            }

            return node;
        }

        public static QsiTableNode VisitSetOperation(SetOperationContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiCompositeTableNode>(context);

            var left = VisitQueryTerm(context.left);
            var right = VisitQueryTerm(context.right);

            node.CompositeType = context.@operator.Text;

            if (context.setQuantifier() is not null)
                node.CompositeType += $" {context.setQuantifier().GetText()}";

            node.Sources.Add(left);
            node.Sources.Add(right);

            return node;
        }

        public static AthenaSetQuantifier VisitSetQuantifier(SetQuantifierContext context)
        {
            return context.HasToken(ALL)
                ? AthenaSetQuantifier.All
                : AthenaSetQuantifier.Distinct;
        }

        #region Select Item
        public static QsiColumnNode VisitSelectItem(SelectItemContext context)
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

        public static QsiColumnNode VisitSelectSingle(SelectSingleContext context)
        {
            var exprNode = ExpressionVisitor.VisitExpression(context.expression());
            var node = exprNode.ToColumn();

            if (context.TryGetRuleContext<IdentifierContext>(out var identifierContext))
            {
                var derivedNode = node.ToDerivedColumn(context);
                derivedNode.Alias.Value = new QsiAliasNode { Name = identifierContext.qi };

                node = derivedNode;
            }

            return node;
        }

        public static QsiColumnNode VisitSelectAll(SelectAllContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiAllColumnNode>(context);
            node.Path = context.qualifiedName()?.qqi;

            return node;
        }
        #endregion

        public static QsiTableDirectivesNode VisitWithClause(WithContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiTableDirectivesNode>(context);
            node.Tables.AddRange(context.namedQuery().Select(VisitNamedQuery));

            return node;
        }

        public static QsiDerivedTableNode VisitNamedQuery(NamedQueryContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiDerivedTableNode>(context);

            node.Alias.Value = new QsiAliasNode { Name = context.name.qi };

            var columnAliases = context.columnAliases();

            if (columnAliases is not null)
            {
                node.Columns.Value = new QsiColumnsDeclarationNode();

                node.Columns.Value.Columns.AddRange(
                    columnAliases.identifier().Select(i => new QsiColumnReferenceNode
                    {
                        Name = new QsiQualifiedIdentifier(i.qi)
                    })
                );
            }

            node.Source.Value = VisitQuery(context.query());

            return node;
        }

        #region Relation
        public static QsiTableNode VisitRelation(RelationContext context)
        {
            switch (context)
            {
                case JoinRelationContext joinRelation:
                    return VisitJoinRelation(joinRelation);

                case RelationDefaultContext relationDefault:
                    return VisitSampledRelation(relationDefault.sampledRelation());

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableNode VisitJoinRelation(JoinRelationContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiJoinedTableNode>(context);
            node.Left.Value = VisitRelation(context.left);

            IEnumerable<IParseTree> joinTypeTrees = context.joinType()?.children ??
                                                    Enumerable.Empty<IParseTree>();

            string joinType = string.Join(" ", joinTypeTrees.Select(c => c.GetText()));

            bool hasJoinType = string.IsNullOrEmpty(joinType);

            if (context.HasToken(CROSS))
            {
                node.JoinType = "CROSS JOIN";
                node.Right.Value = VisitSampledRelation(context.right);
            }
            else if (context.HasToken(NATURAL))
            {
                node.IsNatural = true;
                node.JoinType = hasJoinType ? "NATURAL JOIN" : $"NATURAL {joinType} JOIN";
                node.Right.Value = VisitSampledRelation(context.right);
            }
            else
            {
                node.JoinType = hasJoinType ? "JOIN" : $"{joinType} JOIN";
                node.Right.Value = VisitRelation(context.rightRelation);
            }

            return node;
        }

        public static QsiTableNode VisitSampledRelation(SampledRelationContext context)
        {
            // Ignored tablesample

            return VisitAliasedRelation(context.aliasedRelation());
        }

        public static QsiTableNode VisitAliasedRelation(AliasedRelationContext context)
        {
            var node = VisitRelationPrimary(context.relationPrimary());

            // Relation Alias
            if (!context.TryGetRuleContext<IdentifierContext>(out var identifierContext))
                return node;

            var derivedTableNode = node.ToDerivedTable(context);
            derivedTableNode.Alias.Value = new QsiAliasNode { Name = identifierContext.qi };

            // Column Aliases
            if (!context.TryGetRuleContext<ColumnAliasesContext>(out var columnAliasesContext))
                return derivedTableNode;

            var columnAliases = AthenaTree.CreateWithSpan<QsiColumnsDeclarationNode>(columnAliasesContext);

            foreach (var columnAlias in columnAliasesContext.identifier())
            {
                var sequentialNode = AthenaTree.CreateWithSpan<QsiSequentialColumnNode>(columnAlias);
                sequentialNode.ColumnType = QsiSequentialColumnType.Overwrite;
                sequentialNode.Alias.Value = new QsiAliasNode { Name = columnAlias.qi };

                columnAliases.Columns.Add(sequentialNode);
            }

            derivedTableNode.Columns.Value = columnAliases;
            return derivedTableNode;
        }

        public static QsiTableNode VisitRelationPrimary(RelationPrimaryContext context)
        {
            switch (context)
            {
                case TableNameContext tableName:
                    return VisitQualifiedName(tableName.qualifiedName());

                case SubqueryRelationContext subqueryRelation:
                    return VisitQuery(subqueryRelation.query());

                case UnnestContext unnest:
                    return VisitUnnest(unnest);

                case LateralContext lateral:
                    return VisitLateral(lateral);

                case ParenthesizedRelationContext parenthesizedRelation:
                    return VisitRelation(parenthesizedRelation.relation());

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiTableNode VisitUnnest(UnnestContext _)
        {
            throw TreeHelper.NotSupportedFeature("Unnest table");
        }

        public static QsiTableNode VisitLateral(LateralContext context)
        {
            var node = AthenaTree.CreateWithSpan<AthenaLateralTableNode>(context);
            node.Source.Value = VisitQuery(context.query());

            return node;
        }
        #endregion

        public static QsiTableReferenceNode VisitQualifiedName(QualifiedNameContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiTableReferenceNode>(context);
            node.Identifier = context.qqi;

            return node;
        }

        private static QsiTableNode CreateSource(RelationContext[] relations)
        {
            var source = VisitRelation(relations[0]);

            if (relations.Length != 1)
            {
                for (int i = 1; i < relations.Length; i++)
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
                derivedTable.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            }
            else
            {
                AthenaTree.PutContextSpan(node, context);
            }

            return derivedTable;
        }
        #endregion
    }
}

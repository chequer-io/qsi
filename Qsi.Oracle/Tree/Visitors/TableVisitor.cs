using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Oracle.Common;
using Qsi.Oracle.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class TableVisitor
    {
        public static QsiTableNode VisitSelect(SelectContext context)
        {
            var node = VisitSubquery(context.subquery());

            // TODO: forUpdateClause

            return node;
        }

        public static QsiTableNode VisitSubquery(SubqueryContext context)
        {
            return context switch
            {
                QueryBlockSubqueryContext queryBlocksubquery => VisitQueryBlockSubquery(queryBlocksubquery),
                // JoinedSubqueryContext joinedSubquery => VisitJoinedSubquery(joinedSubquery),
                // ParenthesisSubqueryContext parenthesisSubquery => VisitParenthesisSubquery(parenthesisSubquery),
                _ => throw new NotSupportedException()
            };
        }

        public static QsiTableNode VisitQueryBlockSubquery(QueryBlockSubqueryContext queryBlockSubquery)
        {
            var node = VisitQueryBlock(queryBlockSubquery.queryBlock());

            var orderByClause = queryBlockSubquery.orderByClause();

            if (orderByClause is not null)
            {
                var multipleOrderExpressionNode = OracleTree.CreateWithSpan<OracleMultipleOrderExpressionNode>(orderByClause);

                multipleOrderExpressionNode.IsSiblings = orderByClause.HasToken(SIBLINGS);
                multipleOrderExpressionNode.Orders.AddRange(orderByClause._items.Select(VisitOrderByItem));

                node.Order.Value = multipleOrderExpressionNode;
            }

            // TODO: rowOffset, rowFetchOption

            return node;
        }

        public static QsiOrderExpressionNode VisitOrderByItem(OrderByItemContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleOrderExpressionNode>(context);
            node.Expression.Value = ExpressionVisitor.VisitExpr(context.expr());

            if (context.order != null)
                node.Order = context.order.Type == DESC ? QsiSortOrder.Descending : QsiSortOrder.Ascending;

            if (context.nullsOrder != null)
                node.NullsOrder = context.nullsOrder.Type == FIRST
                    ? OracleNullsOrder.First
                    : OracleNullsOrder.Last;

            return node;
        }

        public static QsiDerivedTableNode VisitQueryBlock(QueryBlockContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiDerivedTableNode>(context);
            node.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.selectList());

            foreach (var selectItem in ExpressionVisitor.VisitSelectList(context.selectList()))
            {
                node.Columns.Value.Columns.Add(selectItem);
            }

            if (context._tables.Count == 1)
            {
                node.Source.Value = VisitTableSource(context._tables[0]);
            }
            else
            {
                var source = VisitTableSource(context._tables[0]);

                for (int i = 1; i < context._tables.Count; i++)
                {
                    var leftContext = context._tables[i - 1];
                    var rightContext = context._tables[i];

                    var joinedTable = OracleTree.CreateWithSpan<QsiJoinedTableNode>(leftContext.Start, rightContext.Stop);

                    joinedTable.IsComma = true;
                    joinedTable.Left.Value = source;
                    joinedTable.Right.Value = VisitTableSource(rightContext);

                    source = joinedTable;
                }

                node.Source.Value = source;
            }

            var withClause = context.withClause();

            if (withClause is not null)
                node.Directives.Value = VisitWithClause(withClause);

            var whereClause = context.whereClause();

            if (whereClause is not null)
            {
                var whereNode = OracleTree.CreateWithSpan<QsiWhereExpressionNode>(whereClause);
                whereNode.Expression.Value = ExpressionVisitor.VisitCondition(whereClause.condition());

                node.Where.Value = whereNode;
            }

            var groupByClause = context.groupByClause();

            if (groupByClause is not null)
            {
                var groupingNode = OracleTree.CreateWithSpan<QsiGroupingExpressionNode>(groupByClause);
                groupingNode.Items.AddRange(groupByClause.groupByItems().groupByItem().Select(VisitGroupByItem));
                var groupingByHavingClause = groupByClause.groupByHavingClause();

                if (groupingByHavingClause is not null)
                    groupingNode.Having.Value = ExpressionVisitor.VisitCondition(groupingByHavingClause.condition());

                node.Grouping.Value = groupingNode;
            }

            // hierarchicalQueryClause, modelClause, windowClause ignored

            return node;
        }

        public static QsiExpressionNode VisitGroupByItem(GroupByItemContext context)
        {
            return context.children[0] switch
            {
                ExprContext expr => ExpressionVisitor.VisitExpr(expr),
                RollupCubeClauseContext rollupCubeClause => throw new NotImplementedException(),
                GroupingSetsClauseContext groupingSetsClause => throw new NotImplementedException(),
                _ => throw new NotSupportedException()
            };
        }

        public static QsiTableNode VisitTableSource(TableSourceContext context)
        {
            while (context is ParenthesisTableSourceContext parenthesisSource)
                context = parenthesisSource.tableSource();

            return context switch
            {
                TableOrJoinTableSourceContext tableOrJoinTableSource => VisitTableOrJoinTableSource(tableOrJoinTableSource),
                InlineAnalyticViewTableSourceContext inlineAnalyticViewTableSource => throw new NotImplementedException(),
                _ => throw new InvalidOperationException()
            };
        }

        public static QsiTableNode VisitTableOrJoinTableSource(TableOrJoinTableSourceContext context)
        {
            var node = VisitTableReference(context.tableReference());

            return node;
        }

        public static QsiTableNode VisitInlineAnalyticView(InlineAnalyticViewContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiTableNode VisitTableReference(TableReferenceContext context)
        {
            return context switch
            {
                QueryTableReferenceContext queryTableReference => VisitQueryTableReference(queryTableReference),
                _ => throw new NotImplementedException()
            };
        }

        public static QsiTableNode VisitQueryTableReference(QueryTableReferenceContext context)
        {
            var node = VisitQueryTableExpression(context.queryTableExpression());
            bool isOnly = context.HasToken(ONLY);

            // TODO: Ignored flashbackQueryClause
            // TODO: Ignored pivotClause, unpivotClause, rowPatternClause 

            if (context.tAlias() is not null)
            {
                if (node is not QsiDerivedTableNode derivedTableNode)
                {
                    derivedTableNode = OracleTree.CreateWithSpan<QsiDerivedTableNode>(context);
                    derivedTableNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                    derivedTableNode.Source.Value = node;
                    node = derivedTableNode;
                }

                derivedTableNode.Alias.Value = new QsiAliasNode
                {
                    Name = IdentifierVisitor.VisitIdentifier(context.tAlias().identifier())
                };
            }

            return node;
        }

        public static QsiTableNode VisitQueryTableExpression(QueryTableExpressionContext context)
        {
            return context switch
            {
                ObjectPathTableExpressionContext objectPathTableExpression => VisitObjectPathTableExpression(objectPathTableExpression),
                _ => throw new NotImplementedException()
            };
        }

        public static QsiTableNode VisitObjectPathTableExpression(ObjectPathTableExpressionContext context)
        {
            var reference = new QsiTableReferenceNode
            {
                Identifier = IdentifierVisitor.VisitFullObjectPath(context.fullObjectPath())
            };

            // TODO: Impl partitionExtensionClause, hierarchiesClause, modifiedExternalTable
            // TODO: Impl sampleClause

            return reference;
        }

        public static QsiTableDirectivesNode VisitWithClause(WithClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiTableDirectivesNode>(context);
            node.Tables.AddRange(context._clauses.Select(VisitFactoringClause));
            return node;
        }

        public static QsiDerivedTableNode VisitFactoringClause(FactoringClauseContext context)
        {
            switch (context.children[0])
            {
                case SubqueryFactoringClauseContext subqueryFactoringClause:
                    var node = OracleTree.CreateWithSpan<QsiDerivedTableNode>(context);

                    node.Alias.Value = new QsiAliasNode
                    {
                        Name = IdentifierVisitor.VisitIdentifier(subqueryFactoringClause.identifier())
                    };

                    var columnList = subqueryFactoringClause.columnList();

                    if (columnList is not null)
                    {
                        node.Columns.Value = new QsiColumnsDeclarationNode();
                        node.Columns.Value.Columns.AddRange(IdentifierVisitor.VisitColumnList(columnList));
                    }
                    else
                    {
                        node.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
                    }

                    node.Source.Value = VisitSubquery(subqueryFactoringClause.subquery());

                    // ignored searchClause, cycleClause
                    return node;

                case SubavFactoringClauseContext subavFactoringClause:
                    break;
            }

            throw new NotSupportedException();
        }
    }
}

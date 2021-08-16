using System;
using Qsi.Oracle.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;

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
            // TODO: orderByClause, rowOffset, rowFetchOption

            return node;
        }

        public static QsiTableNode VisitQueryBlock(QueryBlockContext context)
        {
            // var withClause = context.withClause();

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
                throw new NotImplementedException();
            }

            // TODO: Impl
            // whereClause, hierarchicalQueryClause, groupByClause, modelClause, windowClause

            return node;
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
    }
}

using System.Collections.Generic;
using System.Linq;
using Qsi.Athena.Common;
using Qsi.Tree;
using Qsi.Athena.Internal;
using Qsi.Data;
using Qsi.Shared.Extensions;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;
    
    internal static class CommonVisitor
    {
        internal static QsiMultipleOrderExpressionNode VisitOrderBy(OrderByContext context)
        {
            SortItemContext[] sortItems = context.sortItem();
            IEnumerable<QsiOrderExpressionNode> sortItemNodes = sortItems.Select(VisitSortItem);

            var node = AthenaTree.CreateWithSpan<QsiMultipleOrderExpressionNode>(context);
            node.Orders.AddRange(sortItemNodes);

            return node;
        }
        
        internal static QsiOrderExpressionNode VisitSortItem(SortItemContext context)
        {
            var expression = context.expression();
            var ordering = context.ordering;
            var nullOrdering = context.nullOrdering;

            var expressionNode = ExpressionVisitor.VisitExpression(expression);

            var node = AthenaTree.CreateWithSpan<AthenaOrderExpressionNode>(context);
            node.Expression.Value = expressionNode;

            if (ordering is not null)
                node.Order = ordering.Type == ASC
                    ? QsiSortOrder.Ascending
                    : QsiSortOrder.Descending;

            if (context.HasToken(NULLS))
                node.NullBehavior = nullOrdering.Type == FIRST
                    ? AthenaOrderByNullBehavior.NullsFirst
                    : AthenaOrderByNullBehavior.NullsLast;

            return node;
        }
    }
}

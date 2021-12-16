using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Athena.Internal;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode VisitExpression(ExpressionContext context)
        {
            return context.children[0] switch
            {
                BooleanExpressionContext booleanExpressionContext => VisitBooleanExpression(booleanExpressionContext),
                _ => throw TreeHelper.NotSupportedTree(context.children[0])
            };
        }

        public static QsiExpressionNode VisitBooleanExpression(BooleanExpressionContext context)
        {
            return context switch
            {
                PredicatedContext predicatedContext => VisitPredicated(predicatedContext),
                LogicalNotContext logicalNotContext => VisitLogicalNot(logicalNotContext),
                LogicalBinaryContext logicalBinaryContext => VisitLogicalBinary(logicalBinaryContext),
                _ => throw TreeHelper.NotSupportedTree(context)
            };
        }

        private static QsiExpressionNode VisitLogicalBinary(LogicalBinaryContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitBooleanExpression(context.left);
            node.Operator = context.@operator.Text;
            node.Right.Value = VisitBooleanExpression(context.right);

            return node;
        }

        private static QsiExpressionNode VisitLogicalNot(LogicalNotContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiUnaryExpressionNode>(context);

            node.Operator = "NOT";
            node.Expression.Value = VisitBooleanExpression(context.booleanExpression());

            return node;
        }

        private static QsiExpressionNode VisitPredicated(PredicatedContext context)
        {
            var leftNode = VisitValueExpression(context.valueExpression());

            if (context.predicate() is null)
                return leftNode;

            var node = VisitPredicate(context.predicate(), leftNode);
            AthenaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitPredicate(PredicateContext context, QsiExpressionNode leftNode)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitValueExpression(ValueExpressionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitGroupingElement(GroupingElementContext context)
        {
            switch (context)
            {
                case SingleGroupingSetContext singleGroupingSet:
                {
                    return VisitGroupingSet(singleGroupingSet.groupingSet());
                }

                case RollupContext rollup:
                {
                    var invokeNode = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
                    invokeNode.Member.Value = TreeHelper.CreateFunction("ROLLUP");
                    invokeNode.Parameters.AddRange(rollup.expression().Select(VisitExpression));

                    return invokeNode;
                }

                case CubeContext cube:
                {
                    var invokeNode = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
                    invokeNode.Member.Value = TreeHelper.CreateFunction("CUBE");
                    invokeNode.Parameters.AddRange(cube.expression().Select(VisitExpression));

                    return invokeNode;
                }

                case MultipleGroupingSetsContext multipleGroupingSets:
                {
                    var invokeNode = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
                    invokeNode.Member.Value = TreeHelper.CreateFunction(AthenaKnownFunction.GroupingSets);
                    invokeNode.Parameters.AddRange(multipleGroupingSets.groupingSet().Select(VisitGroupingSet));

                    return invokeNode;
                }

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiExpressionNode VisitGroupingSet(GroupingSetContext context)
        {
            if (context.GetText()[0] == '(')
            {
                var multipleExpressionNode = AthenaTree.CreateWithSpan<QsiMultipleExpressionNode>(context);
                multipleExpressionNode.Elements.AddRange(context.expression().Select(VisitExpression));

                return multipleExpressionNode;
            }

            return VisitExpression(context.expression(0));
        }

        public static QsiOrderExpressionNode VisitSortItem(SortItemContext context)
        {
            // TODO: Implement
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiWhereExpressionNode VisitWhere(BooleanExpressionContext context, ITerminalNode whereNode)
        {
            var node = AthenaTree.CreateWithSpan<QsiWhereExpressionNode>(whereNode.Symbol, context.Stop);
            node.Expression.Value = VisitBooleanExpression(context);

            return node;
        }

        public static QsiMultipleOrderExpressionNode CreateMultipleOrderExpression(SortItemContext[] items, ITerminalNode orderNode)
        {
            var node = AthenaTree.CreateWithSpan<QsiMultipleOrderExpressionNode>(orderNode.Symbol, items[^1].Stop);
            node.Orders.AddRange(items.Select(VisitSortItem));

            return node;
        }
    }
}

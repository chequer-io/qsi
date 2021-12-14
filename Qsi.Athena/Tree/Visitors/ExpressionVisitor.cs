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
    }
}

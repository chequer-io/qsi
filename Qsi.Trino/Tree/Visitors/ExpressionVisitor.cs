using Qsi.Data;
using Qsi.Tree;
using Qsi.Trino.Internal;
using Qsi.Utilities;

namespace Qsi.Trino.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ExpressionVisitor
    {
        #region Expressions
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
            throw new System.NotImplementedException();
        }

        private static QsiExpressionNode VisitLogicalNot(LogicalNotContext context)
        {
            throw new System.NotImplementedException();
        }

        private static QsiExpressionNode VisitPredicated(PredicatedContext context)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public static QsiWhereExpressionNode VisitWhere(BooleanExpressionContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiWhereExpressionNode>(context);
            node.Expression.Value = VisitBooleanExpression(context);

            return node;
        }

        public static QsiSetColumnExpressionNode VisitUpdateAssignment(UpdateAssignmentContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiSetColumnExpressionNode>(context);
            node.Target = new QsiQualifiedIdentifier(context.identifier().qi);
            node.Value.Value = VisitExpression(context.expression());

            return node;
        }
    }
}

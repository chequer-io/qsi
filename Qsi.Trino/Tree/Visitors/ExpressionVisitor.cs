using System;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Shared.Extensions;
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
            if (context.HasRule<PredicateContext>())
                throw new NotImplementedException();

            return VisitValueExpression(context.valueExpression());
        }

        public static QsiExpressionNode VisitValueExpression(ValueExpressionContext context)
        {
            switch (context)
            {
                case ValueExpressionDefaultContext valueExpressionDefault:
                    return VisitPrimaryExpression(valueExpressionDefault.primaryExpression());

                case AtTimeZoneContext atTimeZone:
                    throw new NotImplementedException();

                case ArithmeticUnaryContext arithmeticUnary:
                    throw new NotImplementedException();

                case ArithmeticBinaryContext arithmeticBinary:
                    throw new NotImplementedException();

                case ConcatenationContext concatenation:
                    throw new NotImplementedException();

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiExpressionNode VisitPrimaryExpression(PrimaryExpressionContext context)
        {
            switch (context)
            {
                case ColumnReferenceContext columnReference:
                    return VisitColumnReference(columnReference);

                default:
                    throw new NotImplementedException();
            }
        }

        public static QsiExpressionNode VisitColumnReference(ColumnReferenceContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiColumnExpressionNode>(context);

            node.Column.Value = new QsiColumnReferenceNode
            {
                Name = new QsiQualifiedIdentifier(context.identifier().qi)
            };

            return node;
        }

        public static QsiExpressionNode VisitRowCount(RowCountContext context)
        {
            return context.HasToken(INTEGER_VALUE)
                ? TreeHelper.CreateLiteral(long.Parse(context.INTEGER_VALUE().GetText()))
                : VisitParameterExpression(context.parameterExpression());
        }

        private static QsiBindParameterExpressionNode VisitParameterExpression(ParameterExpressionContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiBindParameterExpressionNode>(context);

            node.Index = context.index;
            node.Prefix = "?";
            node.NoSuffix = true;
            node.Type = QsiParameterType.Index;

            return node;
        }
        #endregion

        public static QsiOrderExpressionNode VisitSortItem(SortItemContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoOrderByExpressionNode>(context);

            node.Expression.Value = VisitExpression(context.expression());

            if (context.ordering is not null)
                node.Order = context.ordering.Type == ASC ? QsiSortOrder.Ascending : QsiSortOrder.Descending;

            if (context.HasToken(NULLS))
                node.NullBehavior = context.nullOrdering.Type == FIRST ? TrinoOrderByNullBehavior.NullsFirst : TrinoOrderByNullBehavior.NullsLast;

            return node;
        }

        public static QsiWhereExpressionNode VisitWhere(BooleanExpressionContext context, IToken whereToken)
        {
            var node = TrinoTree.CreateWithSpan<QsiWhereExpressionNode>(whereToken, context.Stop);
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

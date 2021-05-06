using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Hana.Internal.HanaParserInternal;

namespace Qsi.Hana.Tree.Visitors
{
    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode VisitExpression(ExpressionContext context)
        {
            return new QsiBinaryExpressionNode();

            throw new System.NotImplementedException();
        }

        public static QsiExpressionNode VisitAssociationExpression(AssociationExpressionContext context)
        {
            return new QsiBinaryExpressionNode();

            throw new System.NotImplementedException();
        }

        public static QsiExpressionNode VisitCondition(ConditionContext context)
        {
            return new QsiBinaryExpressionNode();

            throw new System.NotImplementedException();
        }

        public static QsiExpressionNode VisitPredicate(PredicateContext context)
        {
            return new QsiBinaryExpressionNode();

            throw new System.NotImplementedException();
        }

        public static HanaCollateExpressionNode VisitCollateClause(CollateClauseContext context)
        {
            var node = new HanaCollateExpressionNode
            {
                Name = context.name.Text
            };

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitUnsignedInteger(IToken token)
        {
            TreeHelper.VerifyTokenType(token, UNSIGNED_INTEGER);

            return new QsiLiteralExpressionNode
            {
                Type = QsiDataType.Numeric,
                Value = ulong.Parse(token.Text)
            };
        }
    }
}

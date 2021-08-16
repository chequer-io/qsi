using System;
using System.Collections.Generic;
using Qsi.Oracle.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class ExpressionVisitor
    {
        public static IEnumerable<QsiColumnNode> VisitSelectList(SelectListContext context)
        {
            if (context.HasToken(MULT_SYMBOL))
                yield return OracleTree.CreateWithSpan<QsiAllColumnNode>(context);

            foreach (var selectListItem in context.selectListItem())
                yield return VisitSelectListItem(selectListItem);
        }

        public static QsiColumnNode VisitSelectListItem(SelectListItemContext context)
        {
            switch (context)
            {
                case ObjectSelectListItemContext objectSelectListItem:
                    var allNode = OracleTree.CreateWithSpan<QsiAllColumnNode>(objectSelectListItem);
                    allNode.Path = IdentifierVisitor.CreateQualifiedIdentifier(objectSelectListItem.identifier());

                    return allNode;

                case ExprSelectListItemContext exprSelectListItem:
                    var node = OracleTree.CreateWithSpan<QsiDerivedColumnNode>(exprSelectListItem);
                    node.Expression.Value = VisitExpr(exprSelectListItem.expr());

                    if (exprSelectListItem.alias() != null)
                    {
                        node.Alias.Value = new QsiAliasNode()
                        {
                            Name = IdentifierVisitor.VisitAlias(exprSelectListItem.alias())
                        };
                    }

                    return node;
            }

            throw new NotSupportedException();
        }

        public static QsiExpressionNode VisitExpr(ExprContext context)
        {
            switch (context)
            {
                case ParenthesisExprContext parenthesisExpr:
                    return VisitParenthesisExpr(parenthesisExpr);

                case SignExprContext signExpr:
                    return VisitSignExpr(signExpr);

                case TimestampExprContext timestampExpr:
                    return VisitTimestampExpr(timestampExpr);

                case BinaryExprContext binaryExpr:
                    return VisitBinaryExpr(binaryExpr);

                case CollateExprContext collateExpr:
                    return VisitCollateExpr(collateExpr);

                case FunctionExprContext functionExpr:
                    return VisitFunctionExpr(functionExpr);

                case CalcMeasExprContext calcMeasExpr:
                    return VisitCalcMeasExpr(calcMeasExpr);

                case CaseExprContext caseExpr:
                    return VisitCaseExpr(caseExpr);

                case CursorExprContext cursorExpr:
                    return VisitCursorExpr(cursorExpr);

                case IntervalExprContext intervalExpr:
                    return VisitIntervalExpr(intervalExpr);

                case ModelExprContext modelExpr:
                    return VisitModelExpr(modelExpr);

                case ObjectAccessExprContext objectAccessExpr:
                    return VisitObjectAccessExpr(objectAccessExpr);

                case PlaceholderExprContext placeholderExpr:
                    return VisitPlaceholderExpr(placeholderExpr);

                case ScalarSubqueryExprContext scalarSubqueryExpr:
                    return VisitScalarSubqueryExpr(scalarSubqueryExpr);

                case TypeConstructorExprContext typeConstructorExpr:
                    return VisitTypeConstructorExpr(typeConstructorExpr);

                case DatetimeExprContext datetimeExpr:
                    return VisitDatetimeExpr(datetimeExpr);

                case SimpleExprContext simpleExpr:
                    return VisitSimpleExpr(simpleExpr);

                case BindVariableExprContext bindVariableExpr:
                    return VisitBindVariableExpr(bindVariableExpr);

                case MultisetExceptExprContext multisetExceptExpr:
                    return VisitMultisetExceptExpr(multisetExceptExpr);

                case ColumnOuterJoinExprContext columnOuterJoinExpr:
                    return VisitColumnOuterJoinExpr(columnOuterJoinExpr);

                default:
                    throw new NotSupportedException();
            }
        }

        public static QsiExpressionNode VisitParenthesisExpr(ParenthesisExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitSignExpr(SignExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitTimestampExpr(TimestampExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitBinaryExpr(BinaryExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCollateExpr(CollateExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitFunctionExpr(FunctionExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCalcMeasExpr(CalcMeasExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCaseExpr(CaseExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCursorExpr(CursorExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitIntervalExpr(IntervalExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitModelExpr(ModelExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitObjectAccessExpr(ObjectAccessExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitPlaceholderExpr(PlaceholderExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitScalarSubqueryExpr(ScalarSubqueryExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitTypeConstructorExpr(TypeConstructorExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitDatetimeExpr(DatetimeExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitSimpleExpr(SimpleExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitBindVariableExpr(BindVariableExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitMultisetExceptExpr(MultisetExceptExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitColumnOuterJoinExpr(ColumnOuterJoinExprContext context)
        {
            throw new NotImplementedException();
        }
    }
}

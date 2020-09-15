using System;
using System.Linq;
using System.Net.Http;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.SqlServer.Tree.Common;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    public class ExpressionVisitor : VisitorBase
    {
        public ExpressionVisitor(IContext context) : base(context)
        {
        }

        #region Scalar Expression
        public QsiExpressionNode VisitScalarExpression(ScalarExpression scalarExpression)
        {
            switch (scalarExpression)
            {
                case BinaryExpression binaryExpression:
                    return VisitBinaryExpression(binaryExpression);

                case ExtractFromExpression extractFromExpression:
                    return VisitExtractFromExpression(extractFromExpression);

                case IdentityFunctionCall identityFunctionCall:
                    return VisitIdentityFunctionCall(identityFunctionCall);

                case OdbcConvertSpecification odbcConvertSpecification:
                    return VisitOdbcConvertSpecification(odbcConvertSpecification);

                case PrimaryExpression primaryExpression:
                    return VisitPrimaryExpression(primaryExpression);

                case ScalarExpressionSnippet scalarExpressionSnippet:
                    return VisitScalarExpressionSnippet(scalarExpressionSnippet);

                case SourceDeclaration sourceDeclaration:
                    return VisitSourceDeclaration(sourceDeclaration);

                case UnaryExpression unaryExpression:
                    return VisitUnaryExpression(unaryExpression);
            }

            throw TreeHelper.NotSupportedTree(scalarExpression);
        }

        private QsiLogicalExpressionNode VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(binaryExpression.FirstExpression));
                n.Right.SetValue(VisitScalarExpression(binaryExpression.SecondExpression));

                n.Operator = binaryExpression.BinaryExpressionType switch
                {
                    BinaryExpressionType.Add => "+",
                    BinaryExpressionType.Divide => "/",
                    BinaryExpressionType.Modulo => "%",
                    BinaryExpressionType.Multiply => "*",
                    BinaryExpressionType.Subtract => "-",
                    BinaryExpressionType.BitwiseAnd => "&",
                    BinaryExpressionType.BitwiseOr => "|",
                    BinaryExpressionType.BitwiseXor => "^",
                    _ => throw new InvalidOperationException()
                };
            });
        }

        private QsiUnaryExpressionNode VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            var expressionType = unaryExpression.UnaryExpressionType switch
            {
                UnaryExpressionType.Positive => "+",
                UnaryExpressionType.Negative => "-",
                UnaryExpressionType.BitwiseNot => "~",
                _ => throw new InvalidOperationException()
            };
            
            return TreeHelper.CreateUnary(expressionType, VisitScalarExpression(unaryExpression.Expression));
        }

        private QsiExpressionNode VisitSourceDeclaration(SourceDeclaration sourceDeclaration)
        {
            throw TreeHelper.NotSupportedFeature("source declaration");
        }

        private QsiExpressionNode VisitScalarExpressionSnippet(ScalarExpressionSnippet scalarExpressionSnippet)
        {
            throw TreeHelper.NotSupportedFeature("expresssion snippet");
        }

        #region Primary Expression
        private QsiExpressionNode VisitPrimaryExpression(PrimaryExpression primaryExpression)
        {
            switch (primaryExpression)
            {
                case AtTimeZoneCall atTimeZoneCall:
                    break;

                case CaseExpression caseExpression:
                    break;

                case CastCall castCall:
                    break;

                case CoalesceExpression coalesceExpression:
                    break;

                case ColumnReferenceExpression columnReferenceExpression:
                    break;

                case ConvertCall convertCall:
                    break;

                case FunctionCall functionCall:
                    return VisitCommonFunctionInvokeContext(new CommonFunctionInvokeContext(functionCall));

                case IIfCall iifCall:
                    break;

                case LeftFunctionCall leftFunctionCall:
                    break;

                case NextValueForExpression nextValueForExpression:
                    break;

                case NullIfExpression nullIfExpression:
                    break;

                case OdbcFunctionCall odbcFunctionCall:
                    break;

                case ParameterlessCall parameterlessCall:
                    break;

                case ParseCall parseCall:
                    break;

                case PartitionFunctionCall partitionFunctionCall:
                    break;

                case RightFunctionCall rightFunctionCall:
                    break;

                case ScalarSubquery scalarSubquery:
                    break;

                case TryCastCall tryCastCall:
                    break;

                case TryConvertCall tryConvertCall:
                    break;

                case TryParseCall tryParseCall:
                    break;

                case UserDefinedTypePropertyAccess userDefinedTypePropertyAccess:
                    break;

                case ValueExpression valueExpression:
                    return VisitValueExpression(valueExpression);
            }

            throw TreeHelper.NotSupportedTree(primaryExpression);
        }

        #region ValueExpression
        private QsiExpressionNode VisitValueExpression(ValueExpression valueExpression)
        {
            switch (valueExpression)
            {
                case GlobalVariableExpression globalVariableExpression:
                    return VisitGlobalVariableExpression(globalVariableExpression);

                case Literal literal:
                    return VisitLiteral(literal);

                case VariableReference variableReference:
                    return VisitVariableReference(variableReference);
            }

            throw TreeHelper.NotSupportedTree(valueExpression);
        }

        #region Literal
        private QsiLiteralExpressionNode VisitLiteral(Literal literal)
        {
            return TreeHelper.Create<QsiLiteralExpressionNode>(n =>
            {
                n.Type = literal.LiteralType switch
                {
                    LiteralType.Binary => QsiLiteralType.Binary,
                    LiteralType.Default => QsiLiteralType.Default,
                    LiteralType.Identifier => QsiLiteralType.String,
                    LiteralType.Integer => QsiLiteralType.Numeric,
                    LiteralType.Max => QsiLiteralType.Unknown,
                    LiteralType.Money => QsiLiteralType.Decimal,
                    LiteralType.Null => QsiLiteralType.Null,
                    LiteralType.Numeric => QsiLiteralType.Numeric,
                    LiteralType.Odbc => QsiLiteralType.Unknown,
                    LiteralType.Real => QsiLiteralType.Decimal,
                    LiteralType.String => QsiLiteralType.String,
                    _ => throw new InvalidOperationException()
                };

                n.Value = literal.Value;
            });
        }

        // TODO: Impl variable
        private QsiExpressionNode VisitVariableReference(VariableReference variableReference)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(variableReference.Name, false));
            });
        }

        // TODO: Impl variable
        private QsiExpressionNode VisitGlobalVariableExpression(GlobalVariableExpression globalVariableExpression)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(globalVariableExpression.Name, false));
            });
        }
        #endregion
        #endregion
        #endregion

        private QsiExpressionNode VisitOdbcConvertSpecification(OdbcConvertSpecification odbcConvertSpecification)
        {
            throw TreeHelper.NotSupportedFeature("odbc convert specification");
        }

        private QsiExpressionNode VisitIdentityFunctionCall(IdentityFunctionCall identityFunctionCall)
        {
            throw new System.NotImplementedException();
        }

        private QsiExpressionNode VisitExtractFromExpression(ExtractFromExpression extractFromExpression)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        private QsiInvokeExpressionNode VisitCommonFunctionInvokeContext(CommonFunctionInvokeContext commonFunctionInvokeContext)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(commonFunctionInvokeContext.FunctionName));
                n.Parameters.AddRange(commonFunctionInvokeContext.Parameters.Select(VisitScalarExpression));
            });
        }
    }
}

using Microsoft.SqlServer.TransactSql.ScriptDom;
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
        private QsiColumnNode VisitScalarExpression(ScalarExpression scalarExpression)
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

        private QsiColumnNode VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            throw new System.NotImplementedException();
        }

        private QsiColumnNode VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            throw new System.NotImplementedException();
        }

        private QsiColumnNode VisitSourceDeclaration(SourceDeclaration sourceDeclaration)
        {
            throw new System.NotImplementedException();
        }

        private QsiColumnNode VisitScalarExpressionSnippet(ScalarExpressionSnippet scalarExpressionSnippet)
        {
            throw new System.NotImplementedException();
        }

        #region Primary Expression
        private QsiColumnNode VisitPrimaryExpression(PrimaryExpression primaryExpression)
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
                    break;

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
                    break;
            }

            throw TreeHelper.NotSupportedTree(primaryExpression);
        }
        #endregion

        private QsiColumnNode VisitOdbcConvertSpecification(OdbcConvertSpecification odbcConvertSpecification)
        {
            throw new System.NotImplementedException();
        }

        private QsiColumnNode VisitIdentityFunctionCall(IdentityFunctionCall identityFunctionCall)
        {
            throw new System.NotImplementedException();
        }

        private QsiColumnNode VisitExtractFromExpression(ExtractFromExpression extractFromExpression)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}

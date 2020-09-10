using System;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;
using Qsi.SqlServer.Extensions;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    public sealed class ExpressionVisitor : VisitorBase
    {
        public ExpressionVisitor(IContext context) : base(context)
        {
        }
        
        public QsiExpressionNode VisitScalarExpression(SqlScalarExpression scalarExpression)
        {
            switch (scalarExpression)
            {
                case SqlBinaryScalarExpression binaryScalarExpression:
                    return VisitBinaryScalarExpression(binaryScalarExpression);

                case SqlLiteralExpression literalExpression:
                    return VisitLiteralExpression(literalExpression);

                case SqlScalarFunctionCallExpression scalarFunctionCallExpression:
                    return VisitScalarFunctionCallExpression(scalarFunctionCallExpression);

                case SqlScalarRefExpression scalarRefExpression:
                    return VisitScalarRefExpression(scalarRefExpression);

                case SqlScalarSubQueryExpression subQueryExpression:
                    return VisitScalarSubQueryExpression(subQueryExpression);

                case SqlScalarVariableRefExpression scalarVariableRefExpression:
                    return VisitScalarVariableRefExpression(scalarVariableRefExpression);

                case SqlGlobalScalarVariableRefExpression globalScalarVariableRefExpression:
                    return VisitGlobalScalarVariableRefExpression(globalScalarVariableRefExpression);
                
                case SqlSimpleCaseExpression simpleCaseExpression:
                    return VisitSimpleCaseExpression(simpleCaseExpression);
                
                case SqlNullScalarExpression nullScalarExpression:
                    return VisitNullScalarExpression(nullScalarExpression);
            }

            throw TreeHelper.NotSupportedTree(scalarExpression);
        }

        private QsiLogicalExpressionNode VisitBinaryScalarExpression(SqlBinaryScalarExpression binaryScalarExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(binaryScalarExpression.Left));
                n.Right.SetValue(VisitScalarExpression(binaryScalarExpression.Right));

                n.Operator = binaryScalarExpression.Operator.ToOperatorString();
            });
        }

        private QsiLiteralExpressionNode VisitLiteralExpression(SqlLiteralExpression literalExpression)
        {
            return TreeHelper.Create<QsiLiteralExpressionNode>(n =>
            {
                n.Type = literalExpression.Type switch
                {
                    LiteralValueType.Binary => QsiLiteralType.Binary,
                    LiteralValueType.Default => QsiLiteralType.Default,
                    LiteralValueType.Image => QsiLiteralType.Binary,
                    LiteralValueType.Integer => QsiLiteralType.Numeric,
                    LiteralValueType.Money => QsiLiteralType.Decimal,
                    LiteralValueType.Null => QsiLiteralType.Null,
                    LiteralValueType.Numeric => QsiLiteralType.Numeric,
                    LiteralValueType.Real => QsiLiteralType.Decimal,
                    LiteralValueType.String => QsiLiteralType.String,
                    LiteralValueType.UnicodeString => QsiLiteralType.String,
                    _ => throw new InvalidOperationException()
                };

                n.Value = literalExpression.Value;
            });
        }

        private QsiColumnAccessExpressionNode VisitScalarRefExpression(SqlScalarRefExpression scalarRefExpression)
        {
            return TreeHelper.Create<QsiColumnAccessExpressionNode>(n =>
            {
                n.Identifier = IdentifierVisitor.VisitMultipartIdentifier(scalarRefExpression.MultipartIdentifier);
            });
        }

        private QsiInvokeExpressionNode VisitScalarFunctionCallExpression(SqlScalarFunctionCallExpression scalarFunctionCallExpression)
        {
            switch (scalarFunctionCallExpression)
            {
                case SqlIdentityFunctionCallExpression identityFunctionCallExpression:
                    return VisitIdentityFunctionCallExpression(identityFunctionCallExpression);

                case SqlBuiltinScalarFunctionCallExpression builtinScalarFunctionCallExpression:
                    return VisitBuiltinScalarFunctionCallExpression(builtinScalarFunctionCallExpression);

                case SqlUserDefinedScalarFunctionCallExpression userDefinedScalarFunctionCallExpression:
                    return VisitUserDefinedScalarFunctionCallExpression(userDefinedScalarFunctionCallExpression);
            }

            throw TreeHelper.NotSupportedTree(scalarFunctionCallExpression);
        }

        private QsiInvokeExpressionNode VisitIdentityFunctionCallExpression(SqlIdentityFunctionCallExpression identityFunctionCallExpression)
        {
            // ignored seed, increment in identityFunctionCallExpression
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(identityFunctionCallExpression.FunctionName));
                
                if (identityFunctionCallExpression.Children.FirstOrDefault() is SqlDataTypeSpecification dataTypeSpecifiaction)
                {
                    n.Parameters.Add(new QsiTypeAccessExpressionNode
                    {
                        Identifier = IdentifierVisitor.VisitMultipartIdentifier(dataTypeSpecifiaction.DataType.ObjectIdentifier)
                    });
                }
            });
        }

        private QsiInvokeExpressionNode VisitBuiltinScalarFunctionCallExpression(SqlBuiltinScalarFunctionCallExpression builtinScalarFunctionCallExpression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunctionAccess(builtinScalarFunctionCallExpression.FunctionName));

                if (builtinScalarFunctionCallExpression.IsStar)
                {
                    n.Parameters.Add(new QsiColumnAccessExpressionNode
                    {
                        IsAll = true
                    });
                }
                else
                {
                    n.Parameters.AddRange(builtinScalarFunctionCallExpression.Arguments.Select(VisitScalarExpression));
                }
            });
        }

        private QsiInvokeExpressionNode VisitUserDefinedScalarFunctionCallExpression(SqlUserDefinedScalarFunctionCallExpression userDefinedScalarFunctionCallExpression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.Create<QsiFunctionAccessExpressionNode>(fn =>
                {
                    fn.Identifier = IdentifierVisitor.VisitMultipartIdentifier(userDefinedScalarFunctionCallExpression.ObjectIdentifier);
                }));

                n.Parameters.AddRange(userDefinedScalarFunctionCallExpression.Arguments.Select(VisitScalarExpression));
            });
        }

        private QsiTableExpressionNode VisitScalarSubQueryExpression(SqlScalarSubQueryExpression scalarSubQueryExpression)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitQueryExpression(scalarSubQueryExpression.QueryExpression));
            });
        }

        // TODO: Impl variable
        private QsiVariableAccessExpressionNode VisitScalarVariableRefExpression(SqlScalarVariableRefExpression scalarVariableRefExpression)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(scalarVariableRefExpression.VariableName, false));
            });
        }

        // TODO: Impl variable
        private QsiVariableAccessExpressionNode VisitGlobalScalarVariableRefExpression(SqlGlobalScalarVariableRefExpression globalScalarVariableRefExpression)
        {
            return TreeHelper.Create<QsiVariableAccessExpressionNode>(n =>
            {
                n.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(globalScalarVariableRefExpression.VariableName, false));
            });
        }
        
        private QsiSwitchExpressionNode VisitSimpleCaseExpression(SqlSimpleCaseExpression simpleCaseExpression)
        {
            return TreeHelper.Create<QsiSwitchExpressionNode>(n =>
            {
                n.Value.SetValue(VisitScalarExpression(simpleCaseExpression.TestExpression));
                
                n.Cases.AddRange(simpleCaseExpression.WhenClauses.Select(VisitSimpleWhenClause));

                n.Cases.Add(TreeHelper.Create<QsiSwitchCaseExpressionNode>(en =>
                {
                    en.Consequent.SetValue(VisitScalarExpression(simpleCaseExpression.ElseExpression));
                }));
            });
        }

        private QsiSwitchCaseExpressionNode VisitSimpleWhenClause(SqlSimpleWhenClause simpleWhenClause)
        {
            return TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
            {
                n.Condition.SetValue(VisitScalarExpression(simpleWhenClause.WhenExpression));
                n.Consequent.SetValue(VisitScalarExpression(simpleWhenClause.ThenExpression));
            });
        }
        
        private QsiExpressionNode VisitNullScalarExpression(SqlNullScalarExpression nullScalarExpression)
        {
            
            return null;
        }
    }
}

using System;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;
using Qsi.SqlServer.Extensions;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode VisitScalarExpression(SqlScalarExpression scalarExpression)
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
            }

            return null;
        }

        public static QsiLogicalExpressionNode VisitBinaryScalarExpression(SqlBinaryScalarExpression binaryScalarExpression)
        {
            return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
            {
                n.Left.SetValue(VisitScalarExpression(binaryScalarExpression.Left));
                n.Right.SetValue(VisitScalarExpression(binaryScalarExpression.Right));

                n.Operator = binaryScalarExpression.Operator.ToOperatorString();
            });
        }

        public static QsiLiteralExpressionNode VisitLiteralExpression(SqlLiteralExpression literalExpression)
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

        public static QsiInvokeExpressionNode VisitScalarFunctionCallExpression(SqlScalarFunctionCallExpression scalarFunctionCallExpression)
        {
            switch (scalarFunctionCallExpression)
            {
                case SqlBuiltinScalarFunctionCallExpression builtinScalarFunctionCallExpression:
                    return VisitBuiltinScalarFunctionCallExpression(builtinScalarFunctionCallExpression);
            }

            return null;
        }

        public static QsiColumnAccessExpressionNode VisitScalarRefExpression(SqlScalarRefExpression scalarRefExpression)
        {
            return TreeHelper.Create<QsiColumnAccessExpressionNode>(n =>
            {
                n.Identifier = IdentifierVisitor.VisitMultipartIdentifier(scalarRefExpression.MultipartIdentifier);
            });
        }

        public static QsiInvokeExpressionNode VisitBuiltinScalarFunctionCallExpression(SqlBuiltinScalarFunctionCallExpression builtinScalarFunctionCallExpression)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.Create<QsiFunctionAccessExpressionNode>(fn =>
                {
                    fn.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(builtinScalarFunctionCallExpression.FunctionName, false));
                }));

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
    }
}

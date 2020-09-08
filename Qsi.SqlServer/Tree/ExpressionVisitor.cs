using System;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;
using Qsi.Tree.Base;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal static class ExpressionVisitor
    {
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
            return null;
        }
    }
}

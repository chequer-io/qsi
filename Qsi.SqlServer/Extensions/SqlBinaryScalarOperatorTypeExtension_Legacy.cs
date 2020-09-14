using System;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;

namespace Qsi.SqlServer.Extensions
{
    internal static class SqlBinaryScalarOperatorTypeExtension_Legacy
    {
        public static string ToOperatorString(this SqlBinaryScalarOperatorType operatorType)
        {
            return operatorType switch
            {
                SqlBinaryScalarOperatorType.Add => "+",
                SqlBinaryScalarOperatorType.Assign => "",
                SqlBinaryScalarOperatorType.Divide => "/",
                SqlBinaryScalarOperatorType.Equals => "=",
                SqlBinaryScalarOperatorType.Modulus => "%",
                SqlBinaryScalarOperatorType.Multiply => "*",
                SqlBinaryScalarOperatorType.None => "",
                SqlBinaryScalarOperatorType.Subtract => "-",
                SqlBinaryScalarOperatorType.BitwiseAnd => "&",
                SqlBinaryScalarOperatorType.BitwiseOr => "|",
                SqlBinaryScalarOperatorType.BitwiseXor => "^",
                SqlBinaryScalarOperatorType.GreaterThan => ">",
                SqlBinaryScalarOperatorType.LessThan => "<",
                SqlBinaryScalarOperatorType.NotEqualTo => "!=",
                SqlBinaryScalarOperatorType.NotGreaterThan => "!>",
                SqlBinaryScalarOperatorType.NotLessThan => "!<",
                SqlBinaryScalarOperatorType.GreaterThanOrEqual => ">=",
                SqlBinaryScalarOperatorType.LessThanOrEqual => "<=",
                _ => throw new InvalidOperationException()
            };
        }
    }
}

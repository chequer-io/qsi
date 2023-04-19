using System;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static PgQuery.AConst;
using Boolean = PgQuery.Boolean;
using String = PgQuery.String;

namespace Qsi.PostgreSql.Tree;

internal partial class PgNodeVisitor
{
    public static QsiLiteralExpressionNode VisitConst(IPgConstNode node)
    {
        return node switch
        {
            Integer integer => Visit(integer),
            Float @float => Visit(@float),
            Boolean boolean => Visit(boolean),
            String @string => Visit(@string),
            BitString bitString => Visit(bitString),
            _ => throw new NotSupportedException($"Not supported const node: '{node.GetType().Name}'")
        };
    }

    public static QsiLiteralExpressionNode Visit(AConst node)
    {
        if (node.Isnull)
            return TreeHelper.CreateNullLiteral();

        return node.ValCase switch
        {
            ValOneofCase.Fval => Visit(node.Fval),
            ValOneofCase.Ival => Visit(node.Ival),
            ValOneofCase.Sval => Visit(node.Sval),
            ValOneofCase.Boolval => Visit(node.Boolval),
            ValOneofCase.Bsval => Visit(node.Bsval),
            _ => throw CreateInternalException($"Not supported 'A_Const.ValOneofCase.{node.ValCase}'.")
        };
    }

    public static QsiLiteralExpressionNode Visit(Integer node)
    {
        return new QsiLiteralExpressionNode
        {
            Value = node.Ival,
            Type = QsiDataType.Numeric
        };
    }

    public static QsiLiteralExpressionNode Visit(Float node)
    {
        return new QsiLiteralExpressionNode
        {
            Value = float.Parse(node.Fval),
            Type = QsiDataType.Numeric
        };
    }

    public static QsiLiteralExpressionNode Visit(Boolean node)
    {
        return new QsiLiteralExpressionNode
        {
            Value = node.Boolval,
            Type = QsiDataType.Boolean
        };
    }

    public static QsiLiteralExpressionNode Visit(String node)
    {
        return new QsiLiteralExpressionNode
        {
            Value = node.Sval,
            Type = QsiDataType.String
        };
    }

    public static QsiLiteralExpressionNode Visit(BitString node)
    {
        return new QsiLiteralExpressionNode
        {
            Value = new PgBinaryString(node.Bsval),
            Type = QsiDataType.Custom
        };
    }
}

using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.PostgreSql.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.PostgreSql.Internal.PostgreSqlParserInternal;

namespace Qsi.PostgreSql.Tree.Visitors;

internal static class ConstantVisitor
{
    public static QsiExpressionNode VisitConstant(ConstantContext context)
    {
        return context.children[0] switch
        {
            SignedIntContext intContext => VisitInt(intContext),
            FloatContext floatContext => VisitFloat(floatContext),
            HexContext hexContext => VisitHex(hexContext),
            BinContext binContext => VisitBin(binContext),
            StrContext stringContext => VisitString(stringContext),
            FunctionNameContext => VisitFunction(context),
            ConstTypeContext => VisitType(context),
            IntervalContext intervalContext => VisitInterval(intervalContext),
            ITerminalNode => VisitTerminalNode(context),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    public static QsiLiteralExpressionNode VisitInt(SignedIntContext context)
    {
        var literal = context.GetText();
        var value = int.Parse(literal);

        var node = new QsiLiteralExpressionNode
        {
            Value = value,
            Type = QsiDataType.Numeric
        };

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiLiteralExpressionNode VisitFloat(FloatContext context)
    {
        var literal = context.GetText();
        var value = float.Parse(literal);

        var node = new QsiLiteralExpressionNode
        {
            Value = value,
            Type = QsiDataType.Decimal
        };

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiLiteralExpressionNode VisitHex(HexContext context)
    {
        var value = context.GetText();

        var node = new QsiLiteralExpressionNode
        {
            Value = value,
            Type = QsiDataType.Hexadecimal
        };

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiLiteralExpressionNode VisitBin(BinContext context)
    {
        var value = context.GetText();

        var node = new QsiLiteralExpressionNode
        {
            Value = value,
            Type = QsiDataType.Binary
        };

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiLiteralExpressionNode VisitString(StrContext context)
    {
        var literal = context.GetText();
        var value = IdentifierUtility.Unescape(literal);

        var node = new QsiLiteralExpressionNode
        {
            Value = value,
            Type = QsiDataType.String
        };

        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiExpressionNode VisitFunction(ConstantContext context)
    {
        var argList = context.argumentList();
        var name = context.functionName().GetText();

        if (TryGetPostgreSqlStringNode(context, name, out var stringNode))
            return stringNode;

        return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
        {
            n.Member.Value = TreeHelper.CreateFunction(name);

            if (argList is not null)
            {
                n.Parameters.AddRange(FunctionVisitor.VisitArgumentList(argList));
                n.Parameters.Add(TreeHelper.Fragment(context.orderByClause().GetInputText()));
            }

            n.Parameters.Add(VisitString(context.str()));

            PostgreSqlTree.PutContextSpan(n, context);
        });
    }

    private static bool TryGetPostgreSqlStringNode(ConstantContext context, string typeName, out QsiExpressionNode node)
    {
        var value = context.str().GetText();

        PostgreSqlStringKind kind;

        switch (typeName.ToLower())
        {
            case "n":
                kind = PostgreSqlStringKind.National;
                break;

            case "char":
                kind = PostgreSqlStringKind.CharString;
                break;

            case "nchar":
                kind = PostgreSqlStringKind.NCharString;
                break;

            case "bpchar":
                kind = PostgreSqlStringKind.BpCharString;
                break;

            case "varchar":
                kind = PostgreSqlStringKind.VarcharString;
                break;

            default:
                node = default;
                return false;
        }

        node = new QsiLiteralExpressionNode
        {
            Value = new PostgreSqlString(kind, value),
            Type = QsiDataType.String
        };

        return true;
    }

    public static QsiExpressionNode VisitType(ConstantContext context)
    {
        var type = context.constType();

        if (type.characterType() != null)
        {
            var characterName = type
                .characterType()
                .characterPrefix()
                .GetText()
                .ToLower();

            if (TryGetPostgreSqlStringNode(context, characterName, out var stringNode))
                return stringNode;
        }

        return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
        {
            n.Member.Value = TreeHelper.CreateFunction(type.GetText());
            n.Parameters.Add(VisitString(context.str()));

            PostgreSqlTree.PutContextSpan(n, context);
        });
    }

    public static QsiInvokeExpressionNode VisitInterval(IntervalContext context)
    {
        return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
        {
            n.Member.Value = TreeHelper.CreateFunction("INTERVAL");

            if (context.signedInt() is { } signedInt)
                n.Parameters.Add(VisitInt(signedInt));

            n.Parameters.Add(VisitString(context.str()));

            if (context.intervalOption() is { } intervalOption)
                n.Parameters.Add(TreeHelper.Fragment(intervalOption.GetInputText()));

            PostgreSqlTree.PutContextSpan(n, context);
        });
    }

    public static QsiLiteralExpressionNode VisitTerminalNode(ConstantContext context)
    {
        if (context.TRUE_P() != null)
        {
            return new QsiLiteralExpressionNode
            {
                Value = true,
                Type = QsiDataType.Boolean
            };
        }

        if (context.FALSE_P() != null)
        {
            return new QsiLiteralExpressionNode
            {
                Value = false,
                Type = QsiDataType.Boolean
            };
        }

        if (context.NULL_P() != null)
        {
            return new QsiLiteralExpressionNode
            {
                Value = null,
                Type = QsiDataType.Null
            };
        }

        throw TreeHelper.NotSupportedTree(context);
    }
}

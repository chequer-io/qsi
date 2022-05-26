using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.PostgreSql.Data;
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
        var qualified = IdentifierVisitor.VisitFunctionName(context.functionName());
        var functionName = qualified[^1].ToString().ToLower();

        return GetPostgreSqlStringNode(context, functionName);
    }

    private static QsiExpressionNode GetPostgreSqlStringNode(ConstantContext context, string name)
    {
        var value = context.str().GetText();
        var kind = name switch
        {
            "n" => PostgreSqlStringKind.National,
            "char" => PostgreSqlStringKind.CharString,
            "nchar" => PostgreSqlStringKind.NCharString,
            "bpchar" => PostgreSqlStringKind.BpCharString,
            "varchar" => PostgreSqlStringKind.VarcharString,
            _ => throw TreeHelper.NotSupportedTree(context)
        };

        return new QsiLiteralExpressionNode
        {
            Value = new PostgreSqlString(kind, value),
            Type = QsiDataType.String
        };
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

            return GetPostgreSqlStringNode(context, characterName);   
        }

        throw TreeHelper.NotSupportedFeature("Non-string constant type");
    }

    public static QsiLiteralExpressionNode VisitInterval(IntervalContext context)
    {
        throw TreeHelper.NotSupportedTree(context);
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

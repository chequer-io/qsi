using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.PostgreSql.Internal.PostgreSqlParserInternal;

namespace Qsi.PostgreSql.Tree.Visitors;

internal static class FunctionVisitor
{
    public static QsiInvokeExpressionNode VisitFunctionExpression(FunctionExpressionContext context)
    {
        if (context.functionCall() == null)
        {
            return VisitCommonFunctionExpression(context.commonFunctionExpression());
        }

        var functionCall = context.functionCall();
        
        var identifier = IdentifierVisitor.VisitFunctionName(functionCall.functionName());
        var functionNode = new QsiFunctionExpressionNode { Identifier = identifier };
        
        var node = new QsiInvokeExpressionNode();
        node.Member.SetValue(functionNode);

        if (functionCall.functionCallArgument() != null)
        {
            node.Parameters.AddRange(VisitFunctionCallArgument(functionCall.functionCallArgument()));
        }
        
        
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiInvokeExpressionNode VisitCommonFunctionExpression(CommonFunctionExpressionContext context)
    {
        var tokens = context.children
            .OfType<ITerminalNode>()
            .TakeWhile(c => c.GetText() != "(");

        var functionName = tokens
            .Select(t => t.GetText())
            .Aggregate((cur, next) => cur + ' ' + next);

        var i = new QsiIdentifier(functionName, false);
        var identifier = new QsiQualifiedIdentifier(i);
        var functionNode = new QsiFunctionExpressionNode { Identifier = identifier };
        
        var node = new QsiInvokeExpressionNode();
        node.Member.SetValue(functionNode);
        node.Parameters.AddRange(GetParameters(tokens.First().Symbol, context));
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static IEnumerable<QsiExpressionNode> VisitFunctionCallArgument(FunctionCallArgumentContext context)
    {
        if (context.STAR() != null)
        {
            var expression = new QsiColumnExpressionNode();
            expression.Column.SetValue(new QsiAllColumnNode());

            PostgreSqlTree.PutContextSpan(expression, context);
            
            return new[] { expression };
        }

        var list = new List<QsiExpressionNode>();

        if (context.argument() != null)
        {
            list.Add(VisitArgument(context.argument()));
        }

        if (context.argumentList() != null)
        {
            list.AddRange(VisitArgumentList(context.argumentList()));
        }

        return list;
    }

    public static IEnumerable<QsiExpressionNode> VisitArgumentList(ArgumentListContext context)
    {
        return context.argument().Select(VisitArgument);
    }

    public static QsiExpressionNode VisitArgument(ArgumentContext context)
    {
        if (context.typeFunctionIdentifier() == null)
        {
            return ExpressionVisitor.VisitExpression(context.expression());
        }

        var node = new QsiBinaryExpressionNode
        {
            Operator = context.COLON_EQUALS()?.GetText() ?? context.EQUALS_GREATER().GetText()
        };
        
        node.Right.SetValue(ExpressionVisitor.VisitExpression(context.expression()));

        var identifier = IdentifierVisitor.VisitIdentifier(context.typeFunctionIdentifier());
        var qualified = new QsiQualifiedIdentifier(identifier);

        var left = new QsiVariableExpressionNode
        {
            Identifier = qualified
        };
        
        PostgreSqlTree.PutContextSpan(left, context.typeFunctionIdentifier());
        
        node.Left.SetValue(left);
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    private static IEnumerable<QsiExpressionNode> GetParameters(IToken symbol, CommonFunctionExpressionContext context)
    {
        switch (symbol.Type)
        {
            // f(x)
            case COLLATION:
                return new[] { ExpressionVisitor.VisitExpression(context.expression(0)) };
            // f(x...)
            case OVERLAY:
            case NULLIF:
                return context.expression().Select(ExpressionVisitor.VisitExpression);
            // f(xList)
            case COALESCE:
            case GREATEST:
            case LEAST:
            case XMLCONCAT:
                return new[] { ExpressionVisitor.VisitExpressionList(context.expressionList()) };
            // f(int?)
            case CURRENT_TIME:
            case CURRENT_TIMESTAMP:
            case LOCALTIME:
            case LOCALTIMESTAMP:
                return context.signedInt() != null ?
                    new[] { ConstantVisitor.VisitInt(context.signedInt()) } :
                    Enumerable.Empty<QsiExpressionNode>();
            // f(x as type)
            case CAST:
            case TREAT:
                return new[]
                {
                    ExpressionVisitor.VisitExpression(context.expression(0)),
                    ExpressionVisitor.VisitType(context.type())
                };
            // f(extractList)
            case EXTRACT:
                return context.extractList() != null ?
                    new[] { VisitExtractList(context.extractList()) } :
                    Enumerable.Empty<QsiExpressionNode>();
            // f(x (, unicodeNormalForm)?)
            case NORMALIZE:
                return VisitNormalize(context);
            // f(x in x)
            case POSITION:
                return context.positionList().expression().Select(ExpressionVisitor.VisitExpression);
            // substring(...)
            case SUBSTRING:
                return context.substringList().expression().Select(ExpressionVisitor.VisitExpression);
            // trim(...)
            case TRIM:
                return VisitTrimList(context.trimList());
            // f
            case CURRENT_DATE:
            case CURRENT_ROLE:
            case CURRENT_USER:
            case SESSION_USER:
            case USER:
            case CURRENT_CATALOG:
            case CURRENT_SCHEMA:
                return Enumerable.Empty<QsiExpressionNode>();
            // XML(...)
            case XMLEXISTS:
            case XMLFOREST:
            case XMLPARSE:
            case XMLPI:
            case XMLROOT:
            case XMLSERIALIZE:
                throw TreeHelper.NotSupportedTree(context);
            default:
                throw TreeHelper.NotSupportedTree(context);
        }
    }

    public static QsiExpressionNode VisitExtractList(ExtractListContext context)
    {
        var argument = VisitExtractArgument(context.extractArgument());
        var expression = ExpressionVisitor.VisitExpression(context.expression());

        var node = new QsiBinaryExpressionNode();
        node.Left.SetValue(argument);
        node.Operator = context.FROM().GetText();
        node.Right.SetValue(expression);
        
        PostgreSqlTree.PutContextSpan(node, context);

        return node;
    }

    public static QsiExpressionNode VisitExtractArgument(ExtractArgumentContext context)
    {
        var identifier = context.identifier() != null ?
            IdentifierVisitor.VisitIdentifier(context.identifier()):
            new QsiIdentifier(context.GetText(), false);

        var qualified = new QsiQualifiedIdentifier(identifier);

        return new QsiVariableExpressionNode
        {
            Identifier = qualified
        };
    }

    public static IEnumerable<QsiExpressionNode> VisitNormalize(CommonFunctionExpressionContext context)
    {
        var expr = ExpressionVisitor.VisitExpression(context.expression(0));
        
        if (context.unicodeNormalForm() == null)
        {
            return new[] { expr };
        }

        var identifier = new QsiIdentifier(context.unicodeNormalForm().GetText(), false);
        var qualified = new QsiQualifiedIdentifier(identifier);

        var node = new QsiVariableExpressionNode
        {
            Identifier = qualified
        };
        
        PostgreSqlTree.PutContextSpan(node, context.unicodeNormalForm());

        return new[] { expr, node };
    }
    
    public static IEnumerable<QsiExpressionNode> VisitTrimList(TrimListContext context)
    {
        return context.expression() != null ?
            new[]
            {
                ExpressionVisitor.VisitExpression(context.expression()),
                ExpressionVisitor.VisitExpressionList(context.expressionList())
            } :
            new[]
            {
                ExpressionVisitor.VisitExpressionList(context.expressionList())
            };
    }
}

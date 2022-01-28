using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Shared;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.Athena.Tree;

internal static class AthenaTree
{
    static AthenaTree()
    {
        Span = new KeyIndexer<Range>(QsiNodeProperties.Span);
    }

    public static KeyIndexer<Range> Span { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static QsiColumnExpressionNode CreateAllColumnExpressionNode()
    {
        return new QsiColumnExpressionNode
        {
            Column =
            {
                Value = new QsiAllColumnNode()
            }
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static QsiColumnExpressionNode CreateColumnExpressionNode(QsiIdentifier identifier)
    {
        return CreateColumnExpressionNode(new QsiQualifiedIdentifier(identifier));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static QsiColumnExpressionNode CreateColumnExpressionNode(QsiQualifiedIdentifier identifier)
    {
        return new QsiColumnExpressionNode
        {
            Column =
            {
                Value = new QsiColumnReferenceNode
                {
                    Name = identifier
                }
            }
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static QsiFunctionExpressionNode CreateFunction(string identifier)
    {
        return new QsiFunctionExpressionNode
        {
            Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(identifier, false))
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static QsiFunctionExpressionNode CreateFunction(QsiQualifiedIdentifier qualifiedIdentifier)
    {
        return new QsiFunctionExpressionNode
        {
            Identifier = qualifiedIdentifier
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T CreateWithSpan<T>(IParserRuleContext ruleContext) where T : IQsiTreeNode, new()
    {
        var node = new T();
        PutContextSpan(node, ruleContext);

        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T CreateWithSpan<T>(ParserRuleContext ruleContext) where T : IQsiTreeNode, new()
    {
        var node = new T();
        PutContextSpan(node, ruleContext);

        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T CreateWithSpan<T>(IToken token) where T : IQsiTreeNode, new()
    {
        var node = new T();
        PutContextSpan(node, token);

        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T CreateWithSpan<T>(IToken start, IToken stop) where T : IQsiTreeNode, new()
    {
        var node = new T();
        PutContextSpan(node, start, stop);

        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void PutContextSpan(IQsiTreeNode node, IParserRuleContext ruleContext)
    {
        PutContextSpan(node, ruleContext.Start, ruleContext.Stop);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void PutContextSpan(IQsiTreeNode node, ParserRuleContext ruleContext)
    {
        PutContextSpan(node, ruleContext.Start, ruleContext.Stop);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void PutContextSpan(IQsiTreeNode node, IToken token)
    {
        PutContextSpan(node, token, token);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void PutContextSpan(IQsiTreeNode node, IToken start, IToken stop)
    {
        var startIndex = Math.Min(start.StartIndex, stop.TokenSource.InputStream.Size - 1);
        var stopIndex = Math.Min(stop.StopIndex + 1, stop.TokenSource.InputStream.Size);

        Span[node] = new Range(startIndex, stopIndex);
    }
}

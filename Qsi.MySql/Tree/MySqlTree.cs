using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Shared;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.MySql.Tree;

internal static class MySqlTree
{
    public static KeyIndexer<Range> Span { get; }

    public static KeyIndexer<bool> IsSimpleParExpr { get; }

    public static KeyIndexer<QsiSensitiveDataType> SensitiveType { get; }

    static MySqlTree()
    {
        Span = new KeyIndexer<Range>(QsiNodeProperties.Span);
        IsSimpleParExpr = new KeyIndexer<bool>(new Key<bool>("node::simple_par_expr"));
        SensitiveType = new KeyIndexer<QsiSensitiveDataType>(new Key<QsiSensitiveDataType>("node::sensitive_type"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void PutContextSpan(IQsiTreeNode node, IParserRuleContext context)
    {
        PutContextSpan(node, context.Start, context.Stop);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void PutContextSpan(IQsiTreeNode node, ParserRuleContext context)
    {
        PutContextSpan(node, context.Start, context.Stop);
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

    public static TNode WithContextSpan<TNode>(this TNode node, IParserRuleContext context) where TNode : IQsiTreeNode
    {
        PutContextSpan(node, context);
        return node;
    }

    public static TNode WithContextSpan<TNode>(this TNode node, ParserRuleContext context) where TNode : IQsiTreeNode
    {
        PutContextSpan(node, context);
        return node;
    }

    public static TNode WithContextSpan<TNode>(this TNode node, IToken token) where TNode : IQsiTreeNode
    {
        PutContextSpan(node, token);
        return node;
    }

    public static TNode WithContextSpan<TNode>(this TNode node, IToken start, IToken stop) where TNode : IQsiTreeNode
    {
        PutContextSpan(node, start, stop);
        return node;
    }
}
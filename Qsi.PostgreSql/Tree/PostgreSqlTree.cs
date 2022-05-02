using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.PostgreSql.Tree;

internal static class PostgreSqlTree
{
    public static KeyIndexer<Range> Span { get; }

    static PostgreSqlTree()
    {
        Span = new KeyIndexer<Range>(QsiNodeProperties.Span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void PutContextSpan(IQsiTreeNode node, IToken start, IToken stop)
    {
        var startIndex = Math.Min(start.StartIndex, stop.TokenSource.InputStream.Size - 1);
        var stopIndex = Math.Min(stop.StopIndex + 1, stop.TokenSource.InputStream.Size);

        Span[node] = new Range(startIndex, stopIndex);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void PutContextSpan(IQsiTreeNode node, ParserRuleContext context) 
        => PutContextSpan(node, context.Start, context.Stop);
}

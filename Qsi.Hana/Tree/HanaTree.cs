using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Shared;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.Hana.Tree
{
    internal static class HanaTree
    {
        public static KeyIndexer<Range> Span { get; }

        static HanaTree()
        {
            Span = new KeyIndexer<Range>(QsiNodeProperties.Span);
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
    }
}

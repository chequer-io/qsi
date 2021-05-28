using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.PrimarSql.Tree
{
    public static class PrimarSqlTree
    {
        public static KeyIndexer<Range> Span { get; }

        private static readonly Key<Range> SpanKey = new("node_span");

        static PrimarSqlTree()
        {
            Span = new KeyIndexer<Range>(SpanKey);
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
            Span[node] = new Range(start.StartIndex, stop.StopIndex + 1);
        }
    }
}

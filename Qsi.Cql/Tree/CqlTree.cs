using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Cql.Tree.Common;
using Qsi.Shared;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.Cql.Tree
{
    internal static class CqlTree
    {
        public static KeyIndexer<Range> Span { get; }

        private static readonly Key<Range> SpanKey = new Key<Range>("node_span");

        static CqlTree()
        {
            Span = new KeyIndexer<Range>(SpanKey);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutContextSpan(IQsiTreeNode node, IParserRuleContext context)
        {
            PutContextSpan(node, context.Start, context.Stop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutContextSpan(IQsiTreeNode node, ParserRuleContext context)
        {
            PutContextSpan(node, context.Start, context.Stop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutContextSpan(IQsiTreeNode node, IToken token)
        {
            PutContextSpan(node, token, token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutContextSpan(IQsiTreeNode node, IToken start, IToken stop)
        {
            Span[node] = new Range(start.StartIndex, stop.StopIndex + 1);
        }
    }
}

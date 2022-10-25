using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Shared;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.MySql.Tree
{
    internal static class MySqlTree
    {
        public static KeyIndexer<Range> Span { get; }

        public static KeyIndexer<bool> IsSimpleParExpr { get; }

        static MySqlTree()
        {
            Span = new KeyIndexer<Range>(QsiNodeProperties.Span);
            IsSimpleParExpr = new KeyIndexer<bool>(new Key<bool>("node::simple_par_expr"));
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

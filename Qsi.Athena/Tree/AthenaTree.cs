using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Shared;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.Athena.Tree
{
    internal static class AthenaTree
    {
        public static KeyIndexer<Range> Span { get; }

        static AthenaTree()
        {
            Span = new KeyIndexer<Range>(QsiNodeProperties.Span);
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
}

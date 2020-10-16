using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.MySql.Tree.Common;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.MySql.Tree
{
    internal static class MySqlTree
    {
        public static readonly Key<Range> SpanKey = new Key<Range>("node_span");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutSpan(IQsiTreeNode node, Range range)
        {
            node.UserData?.PutData(SpanKey, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Range GetSpan(IQsiTreeNode node)
        {
            return node.UserData?.GetData(SpanKey) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutContextSpan(IQsiTreeNode node, ParserRuleContext context)
        {
            PutContextSpan(node, context.Start, context.Stop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PutContextSpan(IQsiTreeNode node, ICommonContext context)
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
            node.UserData?.PutData(SpanKey, new Range(start.StartIndex, stop.StopIndex + 1));
        }
    }
}

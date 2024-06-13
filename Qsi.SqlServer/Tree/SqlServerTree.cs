using System;
using System.Runtime.CompilerServices;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.SqlServer.Tree;

internal static class SqlServerTree
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range GetSpan(IQsiTreeNode node)
    {
        return node.UserData?.GetData(QsiNodeProperties.Span) ?? default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PutFragmentSpan(IQsiTreeNode node, TSqlFragment sqlFragment)
    {
        PutFragmentSpan(
            node,
            sqlFragment.ScriptTokenStream[sqlFragment.FirstTokenIndex],
            sqlFragment.ScriptTokenStream[sqlFragment.LastTokenIndex]
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PutFragmentSpan(IQsiTreeNode node, TSqlParserToken first, TSqlParserToken last)
    {
        node.UserData?.PutData(QsiNodeProperties.Span, new Range(first.Offset, last.Offset + last.Text.Length));
    }
}

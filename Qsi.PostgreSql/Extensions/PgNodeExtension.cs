using System;
using System.Collections.Generic;
using System.Linq;
using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Extensions;

internal static class PgNodeExtension
{
    public static Node ToNode(this IPgNode node)
    {
        return node.ToNode();
    }

    public static IEnumerable<Node> ToNode(this IEnumerable<IPgNode> nodes)
    {
        return nodes.Select(ToNode);
    }

    public static TResult? InvokeWhenNotNull<TNode, TResult>(this QsiTreeNodeProperty<TNode> node, Func<TNode, TResult> func)
        where TNode : QsiTreeNode
        where TResult : IPgNode
    {
        return node.IsEmpty ? default : func(node.Value);
    }

    public static TResult? InvokeWhenNotNull<TNode, TResult>(this TNode? node, Func<TNode, TResult> func)
        where TNode : QsiTreeNode
        where TResult : IPgNode
    {
        return node is null ? default : func(node);
    }
}

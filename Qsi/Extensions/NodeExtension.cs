using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Extensions
{
    public static class NodeExtension
    {
        public static IEnumerable<IQsiTreeNode> Flatten(this IQsiTreeNode node)
        {
            yield return node;

            foreach (var child in node.Children.SelectMany(Flatten))
            {
                yield return child;
            }
        }

        public static IEnumerable<T> FindAscendants<T>(this IQsiTreeNode node)
        {
            var queue = new Queue<IQsiTreeNode>();
            queue.Enqueue(node);

            while (queue.TryDequeue(out var item))
            {
                if (item is T tNode)
                    yield return tNode;

                foreach (var child in item.Children ?? Enumerable.Empty<IQsiTreeNode>())
                    queue.Enqueue(child);
            }
        }

        #region FindDescendant
        public static bool FindDescendant<T1>(this IQsiTreeNode node, out T1 t1)
            where T1 : IQsiTreeNode
        {
            t1 = default;

            Type[] types = { typeof(T1) };

            if (FindDescendantLayout(node, types, out IQsiTreeNode[] nodes))
            {
                t1 = (T1)nodes[0];

                return true;
            }

            return false;
        }

        public static bool FindDescendant<T1, T2>(this IQsiTreeNode node, out T1 t1, out T2 t2)
            where T1 : IQsiTreeNode
            where T2 : IQsiTreeNode
        {
            t1 = default;
            t2 = default;

            Type[] types = { typeof(T1), typeof(T2) };

            if (FindDescendantLayout(node, types, out IQsiTreeNode[] nodes))
            {
                t1 = (T1)nodes[0];
                t2 = (T2)nodes[1];

                return true;
            }

            return false;
        }

        public static bool FindDescendant<T1, T2, T3>(this IQsiTreeNode node, out T1 t1, out T2 t2, out T3 t3)
            where T1 : IQsiTreeNode
            where T2 : IQsiTreeNode
            where T3 : IQsiTreeNode
        {
            t1 = default;
            t2 = default;
            t3 = default;

            Type[] types = { typeof(T1), typeof(T2), typeof(T3) };

            if (FindDescendantLayout(node, types, out IQsiTreeNode[] nodes))
            {
                t1 = (T1)nodes[0];
                t2 = (T2)nodes[1];
                t3 = (T3)nodes[2];

                return true;
            }

            return false;
        }

        public static bool FindDescendant<T1, T2, T3, T4>(this IQsiTreeNode node, out T1 t1, out T2 t2, out T3 t3, out T4 t4)
            where T1 : IQsiTreeNode
            where T2 : IQsiTreeNode
            where T3 : IQsiTreeNode
            where T4 : IQsiTreeNode
        {
            t1 = default;
            t2 = default;
            t3 = default;
            t4 = default;

            Type[] types = { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };

            if (FindDescendantLayout(node, types, out IQsiTreeNode[] nodes))
            {
                t1 = (T1)nodes[0];
                t2 = (T2)nodes[1];
                t3 = (T3)nodes[2];
                t4 = (T4)nodes[3];

                return true;
            }

            return false;
        }

        public static bool FindDescendantLayout(IQsiTreeNode node, Type[] types, out IQsiTreeNode[] nodes)
        {
            nodes = null;

            for (int i = 0; i < types.Length; i++)
            {
                if (node.Parent == null || !types[i].IsInstanceOfType(node.Parent))
                {
                    nodes = null;
                    return false;
                }

                nodes ??= new IQsiTreeNode[types.Length];

                node = node.Parent;
                nodes[i] = node;
            }

            return true;
        }
        #endregion

        #region IQsiInvokeExpressionNode
        public static string GetMemberName(this IQsiInvokeExpressionNode name)
        {
            return name.Member?.Identifier?.ToString();
        }
        #endregion
    }
}

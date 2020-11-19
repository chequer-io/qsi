using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;
using Qsi.Tree.Immutable;

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

        #region ToImmutable
        public static ImmutableDataConflictActionNode ToImmutable(this IQsiDataConflictActionNode node, bool ignoreUserData = false)
        {
            return new ImmutableDataConflictActionNode(
                node.Parent,
                node.Target,
                node.SetValues,
                node.Condition,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDataDeleteActionNode ToImmutable(this IQsiDataDeleteActionNode node, bool ignoreUserData = false)
        {
            return new ImmutableDataDeleteActionNode(
                node.Parent,
                node.Target,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDataInsertActionNode ToImmutable(this IQsiDataInsertActionNode node, bool ignoreUserData = false)
        {
            return new ImmutableDataInsertActionNode(
                node.Parent,
                node.Directives,
                node.Target,
                node.Partitions,
                node.Columns,
                node.Values,
                node.SetValues,
                node.ValueTable,
                node.ConflictBehavior,
                node.ConflictAction,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDataUpdateActionNode ToImmutable(this IQsiDataUpdateActionNode node, bool ignoreUserData = false)
        {
            return new ImmutableDataUpdateActionNode(
                node.Parent,
                node.Target,
                node.SetValues,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDropPrepareActionNode ToImmutable(this IQsiDropPrepareActionNode node, bool ignoreUserData = false)
        {
            return new ImmutableDropPrepareActionNode(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableExecutePrepareActionNode ToImmutable(this IQsiExecutePrepareActionNode node, bool ignoreUserData = false)
        {
            return new ImmutableExecutePrepareActionNode(
                node.Parent,
                node.Identifier,
                node.Variables,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutablePrepareActionNode ToImmutable(this IQsiPrepareActionNode node, bool ignoreUserData = false)
        {
            return new ImmutablePrepareActionNode(
                node.Parent,
                node.Identifier,
                node.Query,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableAllColumnNode ToImmutable(this IQsiAllColumnNode node, bool ignoreUserData = false)
        {
            return new ImmutableAllColumnNode(
                node.Parent,
                node.Path,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableBindingColumnNode ToImmutable(this IQsiBindingColumnNode node, bool ignoreUserData = false)
        {
            return new ImmutableBindingColumnNode(
                node.Parent,
                node.Id,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDeclaredColumnNode ToImmutable(this IQsiDeclaredColumnNode node, bool ignoreUserData = false)
        {
            return new ImmutableDeclaredColumnNode(
                node.Parent,
                node.Name,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDerivedColumnNode ToImmutable(this IQsiDerivedColumnNode node, bool ignoreUserData = false)
        {
            return new ImmutableDerivedColumnNode(
                node.Parent,
                node.Column,
                node.Expression,
                node.Alias,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSequentialColumnNode ToImmutable(this IQsiSequentialColumnNode node, bool ignoreUserData = false)
        {
            return new ImmutableSequentialColumnNode(
                node.Parent,
                node.Alias,
                node.ColumnType,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableMemberAccessExpressionNode ToImmutable(this IQsiMemberAccessExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableMemberAccessExpressionNode(
                node.Parent,
                node.Target,
                node.Member,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableColumnExpressionNode ToImmutable(this IQsiColumnExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableColumnExpressionNode(
                node.Parent,
                node.Column,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableInvokeExpressionNode ToImmutable(this IQsiInvokeExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableInvokeExpressionNode(
                node.Parent,
                node.Member,
                node.Parameters,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableLimitExpressionNode ToImmutable(this IQsiLimitExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableLimitExpressionNode(
                node.Parent,
                node.Limit,
                node.Offset,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableLiteralExpressionNode ToImmutable(this IQsiLiteralExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableLiteralExpressionNode(
                node.Parent,
                node.Value,
                node.Type,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableLogicalExpressionNode ToImmutable(this IQsiLogicalExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableLogicalExpressionNode(
                node.Parent,
                node.Left,
                node.Operator,
                node.Right,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableMultipleExpressionNode ToImmutable(this IQsiMultipleExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableMultipleExpressionNode(
                node.Parent,
                node.Elements,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableMultipleOrderExpressionNode ToImmutable(this IQsiMultipleOrderExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableMultipleOrderExpressionNode(
                node.Parent,
                node.Orders,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableOrderExpressionNode ToImmutable(this IQsiOrderExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableOrderExpressionNode(
                node.Parent,
                node.Order,
                node.Expression,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableParametersExpressionNode ToImmutable(this IQsiParametersExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableParametersExpressionNode(
                node.Parent,
                node.Expressions,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableRowValueExpressionNode ToImmutable(this IQsiRowValueExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableRowValueExpressionNode(
                node.Parent,
                node.ColumnValues,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSetColumnExpressionNode ToImmutable(this IQsiSetColumnExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableSetColumnExpressionNode(
                node.Parent,
                node.Target,
                node.Value,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSetVariableExpressionNode ToImmutable(this IQsiSetVariableExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableSetVariableExpressionNode(
                node.Parent,
                node.Target,
                node.AssignmentKind,
                node.Value,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSwitchCaseExpressionNode ToImmutable(this IQsiSwitchCaseExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableSwitchCaseExpressionNode(
                node.Parent,
                node.Condition,
                node.Consequent,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSwitchExpressionNode ToImmutable(this IQsiSwitchExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableSwitchExpressionNode(
                node.Parent,
                node.Value,
                node.Cases,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableTableExpressionNode ToImmutable(this IQsiTableExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableTableExpressionNode(
                node.Parent,
                node.Table,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableUnaryExpressionNode ToImmutable(this IQsiUnaryExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableUnaryExpressionNode(
                node.Parent,
                node.Operator,
                node.Expression,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableWhereExpressionNode ToImmutable(this IQsiWhereExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableWhereExpressionNode(
                node.Parent,
                node.Expression,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableFunctionExpressionNode ToImmutable(this IQsiFunctionExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableFunctionExpressionNode(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableVariableExpressionNode ToImmutable(this IQsiVariableExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableVariableExpressionNode(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableColumnsDeclarationNode ToImmutable(this IQsiColumnsDeclarationNode node, bool ignoreUserData = false)
        {
            return new ImmutableColumnsDeclarationNode(
                node.Parent,
                node.Columns,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableCompositeTableNode ToImmutable(this IQsiCompositeTableNode node, bool ignoreUserData = false)
        {
            return new ImmutableCompositeTableNode(
                node.Parent,
                node.Sources,
                node.OrderExpression,
                node.LimitExpression,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDerivedTableNode ToImmutable(this IQsiDerivedTableNode node, bool ignoreUserData = false)
        {
            return new ImmutableDerivedTableNode(
                node.Parent,
                node.Directives,
                node.Columns,
                node.Source,
                node.Alias,
                node.WhereExpression,
                node.GroupingExpression,
                node.OrderExpression,
                node.LimitExpression,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableInlineDerivedTableNode ToImmutable(this IQsiInlineDerivedTableNode node, bool ignoreUserData = false)
        {
            return new ImmutableInlineDerivedTableNode(
                node.Parent,
                node.Alias,
                node.Columns,
                node.Rows,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableJoinedTableNode ToImmutable(this IQsiJoinedTableNode node, bool ignoreUserData = false)
        {
            return new ImmutableJoinedTableNode(
                node.Parent,
                node.Left,
                node.JoinType,
                node.Right,
                node.PivotColumns,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableTableAccessNode ToImmutable(this IQsiTableAccessNode node, bool ignoreUserData = false)
        {
            return new ImmutableTableAccessNode(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableTableDirectivesNode ToImmutable(this IQsiTableDirectivesNode node, bool ignoreUserData = false)
        {
            return new ImmutableTableDirectivesNode(
                node.Parent,
                node.Tables,
                node.IsRecursive,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableGroupingExpressionNode ToImmutable(this IQsiGroupingExpressionNode node, bool ignoreUserData = false)
        {
            return new ImmutableGroupingExpressionNode(
                node.Parent,
                node.Items,
                node.Having,
                ignoreUserData ? null : node.UserData);
        }
        #endregion
    }
}

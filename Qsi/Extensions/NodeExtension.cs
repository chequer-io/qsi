﻿using System;
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

        #region IQsi
        public static bool IsAllColumnNode(this IQsiColumnsDeclarationNode node)
        {
            if (node == null || node.Columns.Length != 1)
                return false;

            return node.Columns[0] is IQsiAllColumnNode { Path: null };
        }
        #endregion

        #region ToImmutable
        public static ImmutableDataConflictActionNode ToImmutable(this IQsiDataConflictActionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Target,
                node.SetValues,
                node.Condition,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDataDeleteActionNode ToImmutable(this IQsiDataDeleteActionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Target,
                node.Columns,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDataInsertActionNode ToImmutable(this IQsiDataInsertActionNode node, bool ignoreUserData = false)
        {
            return new(
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
            return new(
                node.Parent,
                node.Target,
                node.Value,
                node.SetValues,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDropPrepareActionNode ToImmutable(this IQsiDropPrepareActionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableExecutePrepareActionNode ToImmutable(this IQsiExecutePrepareActionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Identifier,
                node.Variables,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutablePrepareActionNode ToImmutable(this IQsiPrepareActionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Identifier,
                node.Query,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableAllColumnNode ToImmutable(this IQsiAllColumnNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Path,
                node.SequentialColumns,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableColumnReferenceNode ToImmutable(this IQsiColumnReferenceNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Name,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDerivedColumnNode ToImmutable(this IQsiDerivedColumnNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Column,
                node.Expression,
                node.Alias,
                node.InferredName,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSequentialColumnNode ToImmutable(this IQsiSequentialColumnNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Alias,
                node.ColumnType,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableMemberAccessExpressionNode ToImmutable(this IQsiMemberAccessExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Target,
                node.Member,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableColumnExpressionNode ToImmutable(this IQsiColumnExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Column,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableInvokeExpressionNode ToImmutable(this IQsiInvokeExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Member,
                node.Parameters,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableLimitExpressionNode ToImmutable(this IQsiLimitExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Limit,
                node.Offset,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableLiteralExpressionNode ToImmutable(this IQsiLiteralExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Value,
                node.Type,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableBinaryExpressionNode ToImmutable(this IQsiBinaryExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Left,
                node.Operator,
                node.Right,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableMultipleExpressionNode ToImmutable(this IQsiMultipleExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Elements,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableMultipleOrderExpressionNode ToImmutable(this IQsiMultipleOrderExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Orders,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableOrderExpressionNode ToImmutable(this IQsiOrderExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Order,
                node.Expression,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableParametersExpressionNode ToImmutable(this IQsiParametersExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Expressions,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableRowValueExpressionNode ToImmutable(this IQsiRowValueExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.ColumnValues,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSetColumnExpressionNode ToImmutable(this IQsiSetColumnExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Target,
                node.Value,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSetVariableExpressionNode ToImmutable(this IQsiSetVariableExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Target,
                node.AssignmentKind,
                node.Value,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSwitchCaseExpressionNode ToImmutable(this IQsiSwitchCaseExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Condition,
                node.Consequent,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableSwitchExpressionNode ToImmutable(this IQsiSwitchExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Value,
                node.Cases,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableTableExpressionNode ToImmutable(this IQsiTableExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Table,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableUnaryExpressionNode ToImmutable(this IQsiUnaryExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Operator,
                node.Expression,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableWhereExpressionNode ToImmutable(this IQsiWhereExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Expression,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableFieldExpressionNode ToImmutable(this IQsiFieldExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableFunctionExpressionNode ToImmutable(this IQsiFunctionExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableVariableExpressionNode ToImmutable(this IQsiVariableExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableColumnsDeclarationNode ToImmutable(this IQsiColumnsDeclarationNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Columns,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableCompositeTableNode ToImmutable(this IQsiCompositeTableNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Sources,
                node.Order,
                node.Limit,
                node.CompositeType,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableDerivedTableNode ToImmutable(this IQsiDerivedTableNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Directives,
                node.Columns,
                node.Source,
                node.Alias,
                node.Where,
                node.Grouping,
                node.Order,
                node.Limit,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableInlineDerivedTableNode ToImmutable(this IQsiInlineDerivedTableNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Alias,
                node.Columns,
                node.Rows,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableJoinedTableNode ToImmutable(this IQsiJoinedTableNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Left,
                node.JoinType,
                node.IsNatural,
                node.IsComma,
                node.Right,
                node.PivotColumns,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableTableReferenceNode ToImmutable(this IQsiTableReferenceNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Identifier,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableTableDirectivesNode ToImmutable(this IQsiTableDirectivesNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Tables,
                node.IsRecursive,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableGroupingExpressionNode ToImmutable(this IQsiGroupingExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Items,
                node.Having,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableBindParameterExpressionNode ToImmutable(this IQsiBindParameterExpressionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Type,
                node.Prefix,
                node.NoSuffix,
                node.Name,
                node.Index,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableChangeSearchPathActionNode ToImmutable(this IQsiChangeSearchPathActionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Identifiers,
                ignoreUserData ? null : node.UserData);
        }

        public static ImmutableTableFunctionNode ToImmutable(this IQsiTableFunctionNode node, bool ignoreUserData = false)
        {
            return new(
                node.Parent,
                node.Member,
                node.Parameters,
                ignoreUserData ? null : node.UserData);
        }
        #endregion
    }
}

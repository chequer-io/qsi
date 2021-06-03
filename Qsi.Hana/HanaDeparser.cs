using System;
using System.Text.Json;
using Qsi.Data;
using Qsi.Hana.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Hana
{
    public sealed class HanaDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = HanaTree.Span[node];

            if (Equals(range, default(Range)))
            {
                switch (node)
                {
                    case HanaTableShareLockBehaviorNode shareLock:
                        DeparseHanaTableShareLockBehaviorNode(writer, shareLock, script);
                        break;

                    case HanaTableUpdateBehaviorNode update:
                        DeparseHanaTableUpdateBehaviorNode(writer, update, script);
                        break;

                    case HanaTableSerializeBehaviorNode serialize:
                        DeparseHanaTableSerializeBehaviorNode(writer, serialize, script);
                        break;

                    case HanaTableSystemTimeBehaviorNode systemTime:
                        DeparseHanaTableSystemTimeBehaviorNode(writer, systemTime, script);
                        break;

                    case HanaTableApplicationTimeBehaviorNode applicationTime:
                        DeparseHanaTableApplicationTimeBehaviorNode(writer, applicationTime, script);
                        break;

                    default:
                        base.DeparseTreeNode(writer, node, script);
                        break;
                }

                return;
            }

            writer.Write(script.Script[range]);
        }

        private void DeparseHanaTableShareLockBehaviorNode(ScriptWriter writer, HanaTableShareLockBehaviorNode node, QsiScript script)
        {
            writer.Write("FOR SHARE LOCK");
        }

        private void DeparseHanaTableUpdateBehaviorNode(ScriptWriter writer, HanaTableUpdateBehaviorNode node, QsiScript script)
        {
            writer.Write("FOR UPDATE");

            if (!node.Columns.IsEmpty)
            {
                writer.Write(" (");
                DeparseTreeNode(writer, node.Columns.Value, script);
                writer.Write(')');
            }

            if (node.WaitTime.HasValue)
            {
                writer.WriteSpace();
                writer.Write(node.WaitTime.Value == -1 ? "NOWAIT" : $"WAIT {node.WaitTime}");
            }

            if (node.IgnoreLocked)
                writer.Write(" IGNORE LOCKED");
        }

        private void DeparseHanaTableSerializeBehaviorNode(ScriptWriter writer, HanaTableSerializeBehaviorNode node, QsiScript script)
        {
            writer.Write("FOR ").Write(node.Type == HanaTableSerializeType.Json ? "JSON" : "XML");

            if (node.Options.Count > 0)
            {
                writer.Write(" (");

                writer.WriteJoin(", ", node.Options, (w, n) =>
                {
                    w.Write(IdentifierUtility.Escape(n.Key, EscapeQuotes.Single, EscapeBehavior.TwoTime));
                    w.Write('=');
                    w.Write(IdentifierUtility.Escape(n.Value, EscapeQuotes.Single, EscapeBehavior.TwoTime));
                });

                writer.Write(')');
            }

            if (!string.IsNullOrEmpty(node.ReturnType))
                writer.Write(" RETURNS ").Write(node.ReturnType);
        }

        private void DeparseHanaTableSystemTimeBehaviorNode(ScriptWriter writer, HanaTableSystemTimeBehaviorNode node, QsiScript script)
        {
            writer.Write("FOR SYSTEM_TIME ");

            if (node.FromTo.HasValue)
            {
                writer.Write("FROM ");
                writer.Write(IdentifierUtility.Escape(node.FromTo.Value.Item1, EscapeQuotes.Single, EscapeBehavior.TwoTime));
                writer.Write("TO ");
                writer.Write(IdentifierUtility.Escape(node.FromTo.Value.Item2, EscapeQuotes.Single, EscapeBehavior.TwoTime));
            }
            else if (node.Between.HasValue)
            {
                writer.Write("BETWEEN ");
                writer.Write(IdentifierUtility.Escape(node.Between.Value.Item1, EscapeQuotes.Single, EscapeBehavior.TwoTime));
                writer.Write("AND ");
                writer.Write(IdentifierUtility.Escape(node.Between.Value.Item2, EscapeQuotes.Single, EscapeBehavior.TwoTime));
            }
            else
            {
                writer.Write("AS OF ");
                writer.Write(IdentifierUtility.Escape(node.Time, EscapeQuotes.Single, EscapeBehavior.TwoTime));
            }
        }

        private void DeparseHanaTableApplicationTimeBehaviorNode(ScriptWriter writer, HanaTableApplicationTimeBehaviorNode node, QsiScript script)
        {
            writer.Write("FOR APPLICATION_TIME AS OF ");
            writer.Write(IdentifierUtility.Escape(node.Time, EscapeQuotes.Single, EscapeBehavior.TwoTime));
        }

        protected override void DeparseExpressionNode(ScriptWriter writer, IQsiExpressionNode node, QsiScript script)
        {
            switch (node)
            {
                case HanaCollateExpressionNode collate:
                    DeparseHanaCollateExpressionNode(writer, collate, script);
                    break;

                case HanaAssociationExpressionNode associationExpression:
                    DeparseHanaAssociationExpressionNode(writer, associationExpression, script);
                    break;

                case HanaAssociationReferenceNode associationRef:
                    DeparseHanaAssociationReferenceNode(writer, associationRef, script);
                    break;

                default:
                    base.DeparseExpressionNode(writer, node, script);
                    break;
            }
        }

        private void DeparseHanaCollateExpressionNode(ScriptWriter writer, HanaCollateExpressionNode node, QsiScript script)
        {
            writer.Write(node.Name);
        }

        private void DeparseHanaAssociationExpressionNode(ScriptWriter writer, HanaAssociationExpressionNode node, QsiScript script)
        {
            writer.WriteJoin(".", node.References, (w, r) => DeparseTreeNode(w, r, script));
        }

        private void DeparseHanaAssociationReferenceNode(ScriptWriter writer, HanaAssociationReferenceNode node, QsiScript script)
        {
            writer.Write(node.Identifier);

            if (node.Condition != null)
            {
                writer.Write("[");

                DeparseTreeNode(writer, node.Condition.Value, script);

                if (!string.IsNullOrEmpty(node.Cardinality))
                {
                    writer.WriteSpace();
                    writer.Write(node.Cardinality);
                }

                writer.Write("]");
            }
        }

        protected override void DeparseOrderExpressionNode(ScriptWriter writer, IQsiOrderExpressionNode node, QsiScript script)
        {
            if (node is HanaOrderByExpressionNode hanaOrderBy)
            {
                DeparseTreeNode(writer, node.Expression, script);

                if (!hanaOrderBy.Collate.IsEmpty)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, hanaOrderBy.Collate.Value, script);
                }

                writer.WriteSpace();
                writer.Write(node.Order == QsiSortOrder.Ascending ? "ASC" : "DESC");

                if (hanaOrderBy.NullBehavior.HasValue)
                {
                    var first = hanaOrderBy.NullBehavior == HanaOrderByNullBehavior.NullsFirst;

                    writer.WriteSpace();
                    writer.Write(first ? "NULLS FIRST" : "NULLS LAST");
                }

                return;
            }

            base.DeparseOrderExpressionNode(writer, node, script);
        }

        protected override void DeparseLimitExpressionNode(ScriptWriter writer, IQsiLimitExpressionNode node, QsiScript script)
        {
            if (node is HanaLimitExpressionNode hanaLimit)
            {
                writer.Write("LIMIT ");
                DeparseTreeNode(writer, node.Limit, script);

                if (node.Offset != null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Offset, script);
                }

                if (hanaLimit.TotalRowCount)
                {
                    writer.WriteSpace();
                    writer.Write("TOTAL ROWCOUNT");
                }

                return;
            }

            base.DeparseLimitExpressionNode(writer, node, script);
        }

        protected override void DeparseTableNode(ScriptWriter writer, IQsiTableNode node, QsiScript script)
        {
            switch (node)
            {
                case HanaAssociationTableNode hanaAssociationTableNode:
                    DeparseHanaAssociationTableNode(writer, hanaAssociationTableNode, script);
                    break;

                case HanaTableReferenceNode hanaTableReferenceNode:
                    DeparseHanaTableReferenceNode(writer, hanaTableReferenceNode, script);
                    break;

                case HanaDerivedTableNode hanaDerivedTableNode:
                    DeparseHanaDerivedTableNode(writer, hanaDerivedTableNode, script);
                    break;

                case HanaCaseJoinTableNode hanaCaseJoinTableNode:
                    DeparseHanaCaseJoinTableNode(writer, hanaCaseJoinTableNode, script);
                    break;

                case HanaCaseJoinWhenTableNode hanaCaseJoinWhenTableNode:
                    DeparseHanaCaseJoinWhenTableNode(writer, hanaCaseJoinWhenTableNode, script);
                    break;

                case HanaCaseJoinElseTableNode hanaCaseJoinElseTableNode:
                    DeparseHanaCaseJoinElseTableNode(writer, hanaCaseJoinElseTableNode, script);
                    break;

                case HanaLateralTableNode hanaLateralTableNode:
                    DeparseHanaLateralTableNode(writer, hanaLateralTableNode, script);
                    break;

                default:
                    base.DeparseTableNode(writer, node, script);
                    break;
            }
        }

        private void DeparseHanaAssociationTableNode(ScriptWriter writer, HanaAssociationTableNode node, QsiScript script)
        {
            // tableName
            writer.Write(node.Identifier);

            // [condition]
            if (!node.Condition.IsEmpty)
            {
                writer.Write('[');
                DeparseTreeNode(writer, node.Condition.Value, script);
                writer.Write(']');
            }

            writer.Write(':');

            DeparseTreeNode(writer, node.Expression.Value, script);
        }

        // tableRef
        private void DeparseHanaTableReferenceNode(ScriptWriter writer, HanaTableReferenceNode node, QsiScript script)
        {
            // tableName
            writer.Write(node.Identifier);

            if (!node.Behavior.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Behavior.Value, script);
            }

            // partitionRestriction?
            if (!node.Partition.IsEmpty)
            {
                writer.WriteSpace();
                writer.Write(node.Partition.Value.Value);
            }
        }

        private void DeparseHanaDerivedTableNode(ScriptWriter writer, HanaDerivedTableNode node, QsiScript script)
        {
            if (IsAliasedTableReferenceNode(node))
            {
                DeparseTreeNode(writer, node.Source.Value, script);
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Alias.Value, script);

                if (!node.Sampling.IsEmpty)
                {
                    writer.WriteSpace();
                    writer.Write(node.Sampling.Value.Value);
                }

                return;
            }

            if (!node.Directives.IsEmpty)
            {
                DeparseTreeNode(writer, node.Directives.Value, script);
                writer.WriteSpace();
            }

            writer.Write("SELECT ");

            if (node.Top.HasValue)
                writer.Write($"TOP {node.Top.Value} ");

            if (node.Operation.HasValue)
                writer.Write(node.Operation == HanaResultSetOperation.All ? "ALL " : "DISTINCT ");

            if (!node.Columns.IsEmpty)
                DeparseTreeNode(writer, node.Columns.Value, script);

            if (!node.Source.IsEmpty)
            {
                writer.WriteSpace().Write("FROM ");

                if (node.Source.Value is IQsiDerivedTableNode leftSource && !IsAliasedTableReferenceNode(leftSource) ||
                    node.Source.Value is IQsiCompositeTableNode)
                {
                    DeparseTreeNodeWithParenthesis(writer, node.Source.Value, script);
                }
                else
                {
                    DeparseTreeNode(writer, node.Source.Value, script);
                }
            }

            if (!node.Where.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Where.Value, script);
            }

            if (!node.Grouping.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Grouping.Value, script);
            }

            if (!node.Order.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Order.Value, script);
            }

            if (!node.Limit.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Limit.Value, script);
            }

            if (!node.Behavior.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Behavior.Value, script);
            }

            if (!node.Behavior.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Behavior.Value, script);
            }
            else if (!node.TimeTravel.IsEmpty)
            {
                writer.WriteSpace();
                writer.Write(node.TimeTravel.Value.Value);
            }

            if (!node.Hint.IsEmpty)
            {
                writer.WriteSpace();
                writer.Write(node.Hint.Value.Value);
            }
        }

        private void DeparseHanaCaseJoinTableNode(ScriptWriter writer, HanaCaseJoinTableNode node, QsiScript script)
        {
            DeparseTreeNode(writer, node.Source.Value, script);

            writer.WriteSpace();
            writer.Write("LEFT OUTER MANY TO ONE CASE JOIN ");

            writer.WriteJoin(" ", node.WhenSources, (w, e) => DeparseHanaCaseJoinWhenTableNode(w, e, script));

            if (!node.ElseSource.IsEmpty)
            {
                writer.WriteSpace();
                DeparseHanaCaseJoinElseTableNode(writer, node.ElseSource.Value, script);
            }

            writer.WriteSpace().Write("END");

            if (!node.Alias.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Alias.Value, script);
            }
        }

        private void DeparseHanaCaseJoinWhenTableNode(ScriptWriter writer, HanaCaseJoinWhenTableNode node, QsiScript script)
        {
            writer.Write("WHEN ");
            DeparseTreeNode(writer, node.Condition.Value, script);

            writer.WriteSpace();
            writer.Write("THEN RETURN (");
            writer.WriteJoin(", ", node.Columns.Value.Columns, (w, c) => DeparseTreeNode(w, c, script));
            writer.Write(") FROM ");

            DeparseTreeNode(writer, node.Source.Value, script);

            writer.WriteSpace().Write("ON ");

            DeparseTreeNode(writer, node.Predicate.Value, script);
        }

        private void DeparseHanaCaseJoinElseTableNode(ScriptWriter writer, HanaCaseJoinElseTableNode node, QsiScript script)
        {
            writer.Write("ELSE RETURN (");
            writer.WriteJoin(", ", node.Columns.Value.Columns, (w, c) => DeparseTreeNode(w, c, script));
            writer.Write(") FROM ");

            DeparseTreeNode(writer, node.Source.Value, script);

            writer.WriteSpace().Write("ON ");

            DeparseTreeNode(writer, node.Predicate.Value, script);
        }

        private void DeparseHanaLateralTableNode(ScriptWriter writer, HanaLateralTableNode node, QsiScript script)
        {
            writer.Write("LATERAL (");
            DeparseTreeNode(writer, node.Source.Value, script);
            writer.Write(")");
        }
    }
}

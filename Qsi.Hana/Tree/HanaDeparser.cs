using System;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = HanaTree.Span[node];

            if (Equals(range, default(Range)))
            {
                base.DeparseTreeNode(writer, node, script);
                return;
            }

            writer.Write(script.Script[range]);
        }

        protected override void DeparseTableNode(ScriptWriter writer, IQsiTableNode node, QsiScript script)
        {
            switch (node)
            {
                case HanaTableAccessNode hanaTableAccessNode:
                    DeparseHanaTableAccessNode(writer, hanaTableAccessNode, script);
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

        // tableRef
        private void DeparseHanaTableAccessNode(ScriptWriter writer, HanaTableAccessNode node, QsiScript script)
        {
            // tableName
            writer.Write(node.Identifier);

            // (forSystemTime | forApplicationTimePeriod)?
            if (!string.IsNullOrWhiteSpace(node.ForSystemTime))
            {
                writer.WriteSpace();
                writer.Write(node.ForSystemTime);
            }
            else if (!string.IsNullOrWhiteSpace(node.ForApplicationTime))
            {
                writer.WriteSpace();
                writer.Write(node.ForApplicationTime);
            }

            // partitionRestriction?
            if (!string.IsNullOrWhiteSpace(node.Partition))
            {
                writer.WriteSpace();
                writer.Write(node.Partition);
            }
        }

        private void DeparseHanaDerivedTableNode(ScriptWriter writer, HanaDerivedTableNode node, QsiScript script)
        {
            if (IsAliasedTableAccessNode(node))
            {
                DeparseTreeNode(writer, node.Source.Value, script);
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Alias.Value, script);

                if (!string.IsNullOrWhiteSpace(node.Sampling))
                {
                    writer.WriteSpace();
                    writer.Write(node.Sampling);
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

                if (node.Source.Value is IQsiDerivedTableNode leftSource && !IsAliasedTableAccessNode(leftSource) ||
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
            writer.WriteJoin(", ", node.Columns.Value.Columns);
            writer.Write(") FROM ");

            DeparseTreeNode(writer, node.Source.Value, script);

            writer.WriteSpace().Write("ON ");

            DeparseTreeNode(writer, node.Predicate.Value, script);
        }

        private void DeparseHanaCaseJoinElseTableNode(ScriptWriter writer, HanaCaseJoinElseTableNode node, QsiScript script)
        {
            writer.Write("ELSE RETURN (");
            writer.WriteJoin(", ", node.Columns.Value.Columns);
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

            if (!node.Alias.IsEmpty)
            {
                writer.WriteSpace();
                DeparseTreeNode(writer, node.Alias.Value, script);
            }
        }
    }
}

using System;
using Qsi.Data;
using Qsi.Hana.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.Hana
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

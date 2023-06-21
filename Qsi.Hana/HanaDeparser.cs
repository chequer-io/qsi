using System;
using Qsi.Data;
using Qsi.Hana.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Hana;

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

                case HanaViewDefinitionNode viewDefinition:
                    DeparseHanaViewDefinitionNode(writer, viewDefinition, script);
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

        if (node.WaitTime.IsEmpty)
        {
            writer.WriteSpace();

            if (node.WaitTime.IsEmpty)
            {
                writer.Write("NOWAIT");
            }
            else
            {
                writer.Write("WAIT ");
                DeparseTreeNode(writer, node.WaitTime.Value, script);
            }
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

    private void DeparseHanaViewDefinitionNode(ScriptWriter writer, HanaViewDefinitionNode node, QsiScript script)
    {
        writer.Write("CREATE VIEW ").Write(node.Identifier);

        if (!string.IsNullOrEmpty(node.Comment))
        {
            writer.Write(" COMMENT ");
            writer.Write(IdentifierUtility.Escape(node.Comment, EscapeQuotes.Single, EscapeBehavior.TwoTime));
        }

        if (!node.Columns.IsEmpty)
        {
            writer.WriteSpace();
            writer.Write('(');
            DeparseTreeNode(writer, node.Columns.Value, script);
            writer.Write(')');
        }

        if (!node.Parameters.IsEmpty)
            writer.WriteSpace().Write(node.Parameters.Value.Text);

        writer.Write(" AS ");
        DeparseTreeNode(writer, node.Source.Value, script);

        if (!node.Associations.IsEmpty)
            writer.WriteSpace().Write(node.Associations.Value.Text);

        if (!node.Masks.IsEmpty)
            writer.WriteSpace().Write(node.Masks.Value.Text);

        if (!node.ExpressionMacros.IsEmpty)
            writer.WriteSpace().Write(node.ExpressionMacros.Value.Text);

        if (!node.Annotation.IsEmpty)
            writer.WriteSpace().Write(node.Annotation.Value.Text);

        if (node.StructuredPrivilegeCheck)
            writer.WriteSpace().Write("WITH STRUCTURED PRIVILEGE CHECK");

        if (!node.Cache.IsEmpty)
            writer.WriteSpace().Write(node.Cache.Value.Text);

        if (!node.Force)
            writer.WriteSpace().Write("FORCE");

        if (!node.CheckOption)
            writer.WriteSpace().Write("WITH CHECK OPTION");

        if (!node.DdlOnly)
            writer.WriteSpace().Write("WITH DDL ONLY");

        if (!node.ReadOnly)
            writer.WriteSpace().Write("WITH READ ONLY");

        if (!node.Anonymization.IsEmpty)
            writer.WriteSpace().Write(node.Anonymization.Value.Text);
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

            case IHanaDerivedTableNode hanaDerivedTableNode:
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
            writer.Write(node.Partition.Value.Text);
        }

        // tableSampleClause?
        if (!node.Sampling.IsEmpty)
        {
            writer.WriteSpace();
            writer.Write(node.Sampling.Value.Text);
        }
    }

    private bool IsAliasedTableReferenceNode(IHanaDerivedTableNode node)
    {
        return
            node.Source is IQsiTableReferenceNode &&
            node.Alias is null &&
            node.Directives is null &&
            node.Where is null &&
            node.Grouping is null &&
            node.Order is null &&
            node.Limit is null &&
            (node.Columns == null || IsWildcard(node.Columns)) &&
            node.Top is null &&
            !node.Operation.HasValue &&
            node.Sampling is not null &&
            node.Behavior is not null &&
            node.TimeTravel is not null &&
            node.Hint is not null;
    }

    private void DeparseHanaDerivedTableNode(ScriptWriter writer, IHanaDerivedTableNode node, QsiScript script)
    {
        if (IsAliasedTableReferenceNode(node))
        {
            DeparseTreeNode(writer, node.Source, script);
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Alias, script);

            if (node.Sampling != null)
            {
                writer.WriteSpace();
                writer.Write(node.Sampling.Text);
            }

            return;
        }

        if (node.Directives != null)
        {
            DeparseTreeNode(writer, node.Directives, script);
            writer.WriteSpace();
        }

        writer.Write("SELECT ");

        if (node.Top != null)
        {
            writer.Write("TOP ");
            DeparseTreeNode(writer, node.Top, script);
        }

        if (node.Operation.HasValue)
            writer.Write(node.Operation == HanaResultSetOperation.All ? "ALL " : "DISTINCT ");

        if (node.Columns != null)
            DeparseTreeNode(writer, node.Columns, script);

        if (node.Source != null)
        {
            writer.WriteSpace().Write("FROM ");

            if (node.Source is IQsiDerivedTableNode leftSource && !IsAliasedTableReferenceNode(leftSource) ||
                node.Source is IQsiCompositeTableNode)
            {
                DeparseTreeNodeWithParenthesis(writer, node.Source, script);
            }
            else
            {
                DeparseTreeNode(writer, node.Source, script);
            }
        }

        if (node.Where != null)
        {
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Where, script);
        }

        if (node.Grouping != null)
        {
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Grouping, script);
        }

        if (node.Order != null)
        {
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Order, script);
        }

        if (node.Limit != null)
        {
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Limit, script);
        }

        if (node.Behavior != null)
        {
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Behavior, script);
        }

        if (node.Behavior != null)
        {
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Behavior, script);
        }
        else if (node.TimeTravel != null)
        {
            writer.WriteSpace();
            writer.Write(node.TimeTravel.Text);
        }

        if (node.Hint != null)
        {
            writer.WriteSpace();
            writer.Write(node.Hint.Text);
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
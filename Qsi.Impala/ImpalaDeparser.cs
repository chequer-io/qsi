using System;
using Qsi.Data;
using Qsi.Impala.Common;
using Qsi.Impala.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.Impala;

public sealed class ImpalaDeparser : CommonTreeDeparser
{
    protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
    {
        var range = ImpalaTree.Span[node];

        if (Equals(range, default(Range)))
        {
            base.DeparseTreeNode(writer, node, script);
            return;
        }

        writer.Write(script.Script[range]);
    }

    protected override void DeparseWhereExpressionNode(ScriptWriter writer, IQsiWhereExpressionNode node, QsiScript script)
    {
        if (node is ImpalaWhereExpressionNode impalaWhereExpressionNode)
        {
            DeparseImpalaWhereExpressionNode(writer, impalaWhereExpressionNode, script);
        }
        else
        {
            base.DeparseWhereExpressionNode(writer, node, script);
        }
    }

    private void DeparseImpalaWhereExpressionNode(ScriptWriter writer, ImpalaWhereExpressionNode node, QsiScript script)
    {
        writer.Write("WHERE ");

        if (!string.IsNullOrWhiteSpace(node.PlanHints))
        {
            writer.Write(node.PlanHints);
            writer.WriteSpace();
        }

        DeparseTreeNode(writer, node.Expression.Value, script);
    }

    protected override void DeparseGroupingExpressionNode(ScriptWriter writer, IQsiGroupingExpressionNode node, QsiScript script)
    {
        if (node is ImpalaGroupingExpressionNode groupingExpressionNode)
        {
            DeparseImpalaGroupingExpressionNode(writer, groupingExpressionNode, script);
        }
        else
        {
            base.DeparseGroupingExpressionNode(writer, node, script);
        }
    }

    private void DeparseImpalaGroupingExpressionNode(ScriptWriter writer, ImpalaGroupingExpressionNode node, QsiScript script)
    {
        writer.Write("GROUP BY ");

        bool parens = false;

        switch (node.GroupingSetsType)
        {
            case ImpalaGroupingSetsType.Sets:
                writer.Write("GROUPING SETS(");
                parens = true;
                break;

            case ImpalaGroupingSetsType.Cube:
                writer.Write("CUBE(");
                parens = true;
                break;

            case ImpalaGroupingSetsType.Rollup:
                writer.Write("ROLLUP(");
                parens = true;
                break;
        }

        writer.WriteJoin(", ", node.Items, (_, item) => DeparseTreeNode(writer, item, script));

        if (parens)
            writer.Write(')');
    }

    protected override void DeparseOrderExpressionNode(ScriptWriter writer, IQsiOrderExpressionNode node, QsiScript script)
    {
        if (node is ImpalaOrderExpressionNode impalaOrderExpressionNode)
        {
            DeparseImpalaOrderExpressionNode(writer, impalaOrderExpressionNode, script);
        }
        else
        {
            base.DeparseOrderExpressionNode(writer, node, script);
        }
    }

    private void DeparseImpalaOrderExpressionNode(ScriptWriter writer, ImpalaOrderExpressionNode node, QsiScript script)
    {
        DeparseTreeNode(writer, node.Expression.Value, script);

        writer.WriteSpace();
        writer.Write(node.Order == QsiSortOrder.Ascending ? "ASC" : "DESC");

        if (node.NullsOrder.HasValue)
        {
            writer.WriteSpace();
            writer.Write("NULLS ");
            writer.Write(node.NullsOrder == ImpalaNullsOrder.First ? "FIRST" : "LAST");
        }
    }

    protected override void DeparseTableNode(ScriptWriter writer, IQsiTableNode node, QsiScript script)
    {
        switch (node)
        {
            case ImpalaTableReferenceNode tableReferenceNode:
                DeparseImpalaTableReferenceNode(writer, tableReferenceNode, script);
                break;

            case ImpalaValuesTableNode valuesTableNode:
                DeparseImpalaValuesTableNode(writer, valuesTableNode, script);
                break;

            case ImpalaDerivedTableNode derivedTableNode:
                DeparseImpalaDerivedTableNode(writer, derivedTableNode, script);
                break;

            case ImpalaJoinedTableNode joinedTableNode:
                DeparseImpalaJoinedTableNode(writer, joinedTableNode, script);
                break;

            default:
                base.DeparseTableNode(writer, node, script);
                break;
        }
    }

    private void DeparseImpalaTableReferenceNode(ScriptWriter writer, ImpalaTableReferenceNode node, QsiScript script)
    {
        DeparseTableReferenceNode(writer, node, script);

        if (!string.IsNullOrWhiteSpace(node.TableSample))
        {
            writer.WriteSpace();
            writer.Write(node.TableSample);
        }

        if (!string.IsNullOrWhiteSpace(node.PlanHints))
        {
            writer.WriteSpace();
            writer.Write(node.PlanHints);
        }
    }

    private void DeparseImpalaValuesTableNode(ScriptWriter writer, ImpalaValuesTableNode node, QsiScript script)
    {
        writer.Write("VALUES ");

        writer.WriteJoin(", ", node.Rows, (w, row) =>
        {
            DeparseTreeNode(w, row, script);
        });

        if (!node.Order.IsEmpty)
        {
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Order.Value, script);
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

    private void DeparseImpalaDerivedTableNode(ScriptWriter writer, ImpalaDerivedTableNode node, QsiScript script)
    {
        if (IsAliasedTableReferenceNode(node))
        {
            DeparseTreeNode(writer, node.Source.Value, script);
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Alias.Value, script);

            if (!string.IsNullOrWhiteSpace(node.TableSample))
            {
                writer.WriteSpace();
                writer.Write(node.TableSample);
            }

            if (!string.IsNullOrWhiteSpace(node.PlanHints))
            {
                writer.WriteSpace();
                writer.Write(node.PlanHints);
            }

            return;
        }

        if (!node.Alias.IsEmpty)
        {
            DeparseTreeNodeWithParenthesis(writer, node.Source.Value, script);
            writer.WriteSpace();
            DeparseTreeNode(writer, node.Alias.Value, script);

            if (!string.IsNullOrWhiteSpace(node.TableSample))
            {
                writer.WriteSpace();
                writer.Write(node.TableSample);
            }

            return;
        }

        if (!node.Directives.IsEmpty)
        {
            DeparseTreeNode(writer, node.Directives.Value, script);
            writer.WriteSpace();
        }

        writer.Write("SELECT ");

        if (node.IsDistinct.HasValue)
        {
            writer.Write(node.IsDistinct.Value ? "DISTINCT" : "ALL");
            writer.WriteSpace();
        }

        if (!string.IsNullOrWhiteSpace(node.PlanHints))
        {
            writer.Write(node.PlanHints);
            writer.WriteSpace();
        }

        if (!node.Columns.IsEmpty)
            DeparseTreeNode(writer, node.Columns.Value, script);

        if (!node.Source.IsEmpty)
        {
            writer.WriteSpace();
            writer.Write("FROM ");

            DeparseTreeNode(writer, node.Source.Value, script);
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

        if (!string.IsNullOrWhiteSpace(node.TableSample))
        {
            writer.WriteSpace();
            writer.Write(node.TableSample);
        }
    }

    private void DeparseImpalaJoinedTableNode(ScriptWriter writer, ImpalaJoinedTableNode node, QsiScript script)
    {
        string joinType = node.IsComma ? ", " : $" {node.JoinType?.Trim()} ";

        DeparseTreeNode(writer, node.Left.Value, script);
        writer.Write(joinType);

        if (!string.IsNullOrWhiteSpace(node.PlanHints))
            writer.Write(node.PlanHints).WriteSpace();

        DeparseTreeNode(writer, node.Right.Value, script);

        if (node.PivotColumns is not null)
        {
            writer.Write(" USING ");
            DeparseTreeNodeWithParenthesis(writer, node.PivotColumns.Value, script);
        }
    }

    protected override void DeparseLimitExpressionNode(ScriptWriter writer, IQsiLimitExpressionNode node, QsiScript script)
    {
        if (node.Limit is not null)
        {
            writer.Write("LIMIT ");
            DeparseTreeNode(writer, node.Limit, script);
        }

        if (node.Offset is not null)
        {
            if (node.Limit is not null)
                writer.WriteSpace();

            writer.Write("OFFSET ");
            DeparseTreeNode(writer, node.Offset, script);
        }
    }
}
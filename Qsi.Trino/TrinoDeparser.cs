using System;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Tree;
using Qsi.Trino.Tree;

namespace Qsi.Trino
{
    public sealed class TrinoDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = TrinoTree.Span[node];

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
                case TrinoLateralTableNode trinoLateralTableNode:
                    DeparseTrinoLateralTableNode(writer, trinoLateralTableNode, script);
                    break;

                default:
                    base.DeparseTableNode(writer, node, script);
                    break;
            }
        }

        protected override void DeparseExpressionNode(ScriptWriter writer, IQsiExpressionNode node, QsiScript script)
        {
            switch (node)
            {
                case TrinoWindowExpressionNode windowExpression:
                    DeparseWindowNode(writer, windowExpression, script);
                    return;

                default:
                    base.DeparseExpressionNode(writer, node, script);
                    return;
            }
        }

        protected override void DeparseDerivedTableNode(ScriptWriter writer, IQsiDerivedTableNode node, QsiScript script)
        {
            if (node is TrinoDerivedTableNode trinoNode)
            {
                if (IsAliasedTableReferenceNode(node))
                {
                    // IQsiTableReferenceNode
                    DeparseTreeNode(writer, node.Source, script);
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Alias, script);
                    return;
                }

                if (IsAliasedDerivedTableNode(node))
                {
                    DeparseTreeNodeWithParenthesis(writer, node.Source, script);
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Alias, script);
                    return;
                }

                if (node.Directives is not null)
                {
                    DeparseTreeNode(writer, node.Directives, script);
                    writer.WriteSpace();
                }

                writer.Write("SELECT ");

                if (trinoNode.SetQuantifier is not null)
                    DeparseSetQuantifier(writer, trinoNode.SetQuantifier.Value);

                if (node.Columns is not null)
                    DeparseTreeNode(writer, node.Columns, script);

                if (node.Source is not null)
                {
                    writer.WriteSpace();
                    writer.Write("FROM ");
                    DeparseTreeNode(writer, node.Source, script);
                }

                if (node.Where is not null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Where, script);
                }

                if (node.Grouping is not null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Grouping, script);
                }

                if (node.Order is not null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Order, script);
                }

                if (node.Limit is not null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Limit, script);
                }

                if (!trinoNode.Window.IsEmpty)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, trinoNode.Window.Value, script);
                }

                return;
            }

            base.DeparseDerivedTableNode(writer, node, script);
        }

        private void DeparseTrinoLateralTableNode(ScriptWriter writer, TrinoLateralTableNode node, QsiScript script)
        {
            writer.Write("LATERAL (");
            DeparseTreeNode(writer, node.Source.Value, script);
            writer.Write(")");
        }

        private void DeparseWindowNode(ScriptWriter writer, TrinoWindowExpressionNode node, QsiScript script)
        {
            writer.Write("WINDOW ");

            writer.WriteJoin(", ", node.Items, (w, item) =>
            {
                DeparseTreeNode(w, item, script);
            });
        }

        private static void DeparseSetQuantifier(ScriptWriter writer, TrinoSetQuantifier setQuantifier)
        {
            writer.Write(setQuantifier.ToString().ToUpperInvariant());
        }
    }
}

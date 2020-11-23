using System;
using Qsi.Cql.Tree;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.Cql
{
    public sealed class CqlDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = CqlTree.Span[node];

            if (Equals(range, default(Range)))
                base.DeparseTreeNode(writer, node, script);

            writer.Write(script.Script[range]);
        }

        protected override void DeparseDerivedTableNode(ScriptWriter writer, IQsiDerivedTableNode node, QsiScript script)
        {
            if (node is CqlDerivedTableNode cqlNode)
            {
                writer.Write("SELECT ");

                if (cqlNode.IsJson)
                    writer.Write("JSON ");

                if (node.Columns != null)
                    DeparseTreeNode(writer, node.Columns, script);

                if (node.Source != null)
                {
                    writer.WriteSpace();
                    writer.Write("FROM ");

                    if (node.Source is IQsiDerivedTableNode leftSource && !IsAliasedTableAccessNode(leftSource) ||
                        node.Source is IQsiCompositeTableNode)
                    {
                        DeparseTreeNodeWithParenthesis(writer, node.Source, script);
                    }
                    else
                    {
                        DeparseTreeNode(writer, node.Source, script);
                    }
                }

                if (node.WhereExpression != null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.WhereExpression, script);
                }

                if (node.GroupingExpression != null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.GroupingExpression, script);
                }

                if (node.OrderExpression != null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.OrderExpression, script);
                }

                if (node.LimitExpression != null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.LimitExpression, script);
                }
            }

            base.DeparseDerivedTableNode(writer, node, script);
        }
    }
}

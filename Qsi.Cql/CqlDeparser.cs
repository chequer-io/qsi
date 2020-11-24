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

                if (cqlNode.IsDistinct)
                    writer.Write("DISTINCT ");

                if (node.Columns != null)
                    DeparseTreeNode(writer, node.Columns, script);

                writer.WriteSpace();
                writer.Write("FROM ");

                DeparseTreeNode(writer, node.Source, script);

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

                if (!cqlNode.PerPartitionLimit.IsEmpty)
                {
                    writer.WriteSpace();
                    writer.Write("PER PARTITION LIMIT ");
                    DeparseTreeNode(writer, cqlNode.PerPartitionLimit.Value, script);
                }

                if (node.Limit != null)
                {
                    writer.WriteSpace();
                    DeparseTreeNode(writer, node.Limit, script);
                }

                if (cqlNode.AllowFiltering)
                {
                    writer.WriteSpace();
                    writer.Write("ALLOW FILTERING");
                }

                return;
            }

            base.DeparseDerivedTableNode(writer, node, script);
        }
    }
}

using System;
using Qsi.Athena.Tree;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.Athena;

public class AthenaDeparser : CommonTreeDeparser
{
    protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
    {
        var range = AthenaTree.Span[node];

        if (Equals(range, default(Range)))
        {
            base.DeparseTreeNode(writer, node, script);
            return;
        }

        writer.Write(script.Script[range]);
    }

    protected override void DeparseDerivedTableNode(ScriptWriter writer, IQsiDerivedTableNode node, QsiScript script)
    {
        if (node is AthenaDerivedTableNode athenaNode)
        {
            writer.Write("SELECT ");

            if (athenaNode.SetQuantifier is not null)
            {
                writer.Write(athenaNode.SetQuantifier.Value.ToString().ToUpperInvariant());
                writer.WriteSpace();
            }

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

            return;
        }

        base.DeparseDerivedTableNode(writer, node, script);
    }

    protected override void DeparseTableNode(ScriptWriter writer, IQsiTableNode node, QsiScript script)
    {
        switch (node)
        {
            case AthenaLateralTableNode athenaLateralTableNode:
                DeparseAthenaLateralTableNode(writer, athenaLateralTableNode, script);
                break;

            default:
                base.DeparseTableNode(writer, node, script);
                break;
        }
    }

    private void DeparseAthenaLateralTableNode(ScriptWriter writer, AthenaLateralTableNode node, QsiScript script)
    {
        writer.Write("LATERAL (");
        DeparseTreeNode(writer, node.Source.Value, script);
        writer.Write(")");
    }
}

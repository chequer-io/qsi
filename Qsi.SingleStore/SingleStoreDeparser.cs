using System;
using Qsi.Data;
using Qsi.SingleStore.Data;
using Qsi.SingleStore.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.SingleStore;

public sealed class SingleStoreDeparser : CommonTreeDeparser
{
    protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
    {
        var range = SingleStoreTree.Span[node];

        if (Equals(range, default(Range)))
        {
            base.DeparseTreeNode(writer, node, script);
            return;
        }

        writer.Write(script.Script[range]);
    }

    protected override void DeparseTableReferenceNode(ScriptWriter writer, IQsiTableReferenceNode node, QsiScript script)
    {
        base.DeparseTableReferenceNode(writer, node, script);

        if (node is SingleStoreTableReferenceNode { Partitions: { Length: > 0 } } tableReferenceNode)
        {
            writer.WriteSpace();
            writer.Write("PARTITION");
            writer.WriteSpace();
            writer.WriteJoin(", ", tableReferenceNode.Partitions);
        }
    }

    protected override void DeparseDerivedTableNode(ScriptWriter writer, IQsiDerivedTableNode node, QsiScript script)
    {
        base.DeparseDerivedTableNode(writer, node, script);

        if (node is not SingleStoreDerivedTableNode singleStoreNode)
            return;

        if (!singleStoreNode.ProcedureAnalyse.IsEmpty)
        {
            var procedureAnalyse = singleStoreNode.ProcedureAnalyse.Value;

            writer.WriteSpace();
            writer.Write("PROCEDURE ANALYSE (");
            writer.Write(procedureAnalyse.MaxElements);

            if (procedureAnalyse.MaxMemory.HasValue)
            {
                writer.Write(", ");
                writer.Write(procedureAnalyse.MaxMemory.Value);
            }

            writer.Write(')');
        }

        if (singleStoreNode.Lockings?.Count > 0)
        {
            writer.WriteSpace();
            writer.WriteJoin(" ", singleStoreNode.Lockings, DeparseLockingNode);
        }
    }

    private void DeparseLockingNode(ScriptWriter writer, SingleStoreLockingNode node)
    {
        if (node.TableLockType == SingleStoreTableLockType.ShareMode)
        {
            writer.Write("LOCK IN SHARE MODE");
        }
        else
        {
            writer.Write("FOR ");
            writer.Write(node.TableLockType == SingleStoreTableLockType.Update ? "UPDATE" : "SHARE");

            if (!ListUtility.IsNullOrEmpty(node.Tables))
            {
                writer.WriteSpace();
                writer.Write("OF ");
                writer.WriteJoin(", ", node.Tables);
            }

            if (node.RowLockType.HasValue)
            {
                writer.WriteSpace();
                writer.Write(node.RowLockType == SingleStoreRowLockType.SkipLocked ? "SKIP LOCKED" : "NOWAIT");
            }
        }
    }
}

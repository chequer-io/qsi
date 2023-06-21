using System;
using Qsi.Data;
using Qsi.MySql.Data;
using Qsi.MySql.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.MySql;

public sealed class MySqlDeparser : CommonTreeDeparser
{
    protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
    {
        var range = MySqlTree.Span[node];

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

        if (node is MySqlTableReferenceNode { Partitions: { Length: > 0 } } tableReferenceNode)
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

        if (node is not MySqlDerivedTableNode mysqlNode)
            return;

        if (!mysqlNode.ProcedureAnalyse.IsEmpty)
        {
            var procedureAnalyse = mysqlNode.ProcedureAnalyse.Value;

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

        if (mysqlNode.Lockings?.Count > 0)
        {
            writer.WriteSpace();
            writer.WriteJoin(" ", mysqlNode.Lockings, DeparseLockingNode);
        }
    }

    private void DeparseLockingNode(ScriptWriter writer, MySqlLockingNode node)
    {
        if (node.TableLockType == MySqlTableLockType.ShareMode)
        {
            writer.Write("LOCK IN SHARE MODE");
        }
        else
        {
            writer.Write("FOR ");
            writer.Write(node.TableLockType == MySqlTableLockType.Update ? "UPDATE" : "SHARE");

            if (!ListUtility.IsNullOrEmpty(node.Tables))
            {
                writer.WriteSpace();
                writer.Write("OF ");
                writer.WriteJoin(", ", node.Tables);
            }

            if (node.RowLockType.HasValue)
            {
                writer.WriteSpace();
                writer.Write(node.RowLockType == MySqlRowLockType.SkipLocked ? "SKIP LOCKED" : "NOWAIT");
            }
        }
    }
}
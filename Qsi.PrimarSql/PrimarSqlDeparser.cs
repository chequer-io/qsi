using System;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.PrimarSql.Tree;
using Qsi.Tree;

namespace Qsi.PrimarSql;

public class PrimarSqlDeparser : CommonTreeDeparser
{
    protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
    {
        var range = PrimarSqlTree.Span[node];

        if (Equals(range, default(Range)))
        {
            base.DeparseTreeNode(writer, node, script);
            return;
        }

        writer.Write(script.Script[range]);
    }
}
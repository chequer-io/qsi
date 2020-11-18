using System;
using Qsi.Cassandra.Tree;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.Cassandra
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
    }
}

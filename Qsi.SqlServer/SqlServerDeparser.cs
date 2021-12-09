using System;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.SqlServer.Data;
using Qsi.SqlServer.Tree;
using Qsi.Tree;

namespace Qsi.SqlServer
{
    public sealed class SqlServerDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = SqlServerTree.GetSpan(node);

            if (Equals(range, default(Range)))
            {
                base.DeparseTreeNode(writer, node, script);
                return;
            }

            writer.Write(script.Script[range]);
        }
    }
}

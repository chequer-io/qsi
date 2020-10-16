using System;
using Qsi.Data;
using Qsi.MySql.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.MySql
{
    public sealed class MySqlDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = MySqlTree.GetSpan(node);

            if (Equals(range, default(Range)))
                base.DeparseTreeNode(writer, node, script);

            writer.Write(script.Script[range]);
        }
    }
}

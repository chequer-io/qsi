using System;
using System.Text;
using Qsi.Data;
using Qsi.MySql.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.MySql
{
    public sealed class MySqlDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(StringBuilder writer, IQsiTreeNode node, QsiScript script)
        {
            var range = MySqlTree.GetSpan(node);

            if (Equals(range, default(Range)))
                base.DeparseTreeNode(writer, node, script);

            writer.Append(script.Script[range]);
        }
    }
}

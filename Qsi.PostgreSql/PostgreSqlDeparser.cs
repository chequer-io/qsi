using System;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.PostgreSql.Tree;
using Qsi.Tree;

namespace Qsi.PostgreSql
{
    public class PostgreSqlDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var range = PostgreSqlTree.Span[node];
            
            if (Equals(range, default(Range)))
            {
                base.DeparseTreeNode(writer, node, script);
                return;
            }

            writer.Write(script.Script[range]);
        }
    }
}

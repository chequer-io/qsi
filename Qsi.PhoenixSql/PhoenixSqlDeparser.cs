using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.PhoenixSql.Tree;
using Qsi.Tree;

namespace Qsi.PhoenixSql
{
    public sealed class PhoenixSqlDeparser : CommonTreeDeparser
    {
        protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        {
            var rawNode = PTree.RawNode[node];

            if (rawNode == null)
            {
                base.DeparseTreeNode(writer, node, script);
                return;
            }

            writer.Write(global::PhoenixSql.PhoenixSqlDeparser.Deparse(rawNode));
        }
    }
}

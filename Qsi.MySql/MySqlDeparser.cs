using Qsi.Parsing.Common;

namespace Qsi.MySql
{
    public sealed class MySqlDeparser : CommonTreeDeparser
    {
        // protected override void DeparseTreeNode(ScriptWriter writer, IQsiTreeNode node, QsiScript script)
        // {
        //     var range = MySqlTree.GetSpan(node);
        //
        //     if (Equals(range, default(Range)))
        //         base.DeparseTreeNode(writer, node, script);
        //
        //     writer.Write(script.Script[range]);
        // }
    }
}

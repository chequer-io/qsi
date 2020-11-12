using Qsi.Tree;

namespace Qsi.PhoenixSql.Internal
{
    internal sealed class PDynamicTableAccessNode : QsiTableAccessNode
    {
        public PDynamicDeclaredColumnNode[] DynamicColumns { get; set; }
    }
}

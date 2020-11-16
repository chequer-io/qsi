using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal sealed class PDynamicTableAccessNode : QsiTableAccessNode
    {
        public QsiColumnsDeclarationNode DynamicColumns { get; set; }
    }
}

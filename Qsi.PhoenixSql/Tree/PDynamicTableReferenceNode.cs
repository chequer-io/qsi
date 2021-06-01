using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal sealed class PDynamicTableReferenceNode : QsiTableReferenceNode, IDynamicTableNode
    {
        public QsiColumnsDeclarationNode DynamicColumns { get; set; }
    }
}

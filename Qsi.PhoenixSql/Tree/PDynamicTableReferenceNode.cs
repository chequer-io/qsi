using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal sealed class PDynamicTableReferenceNode : QsiTableReferenceNode, IDynamicColumnsNode
    {
        public QsiColumnsDeclarationNode DynamicColumns { get; set; }
    }
}

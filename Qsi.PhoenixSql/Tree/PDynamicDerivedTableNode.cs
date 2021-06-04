using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal sealed class PDynamicDerivedTableNode : QsiDerivedTableNode, IDynamicColumnsNode
    {
        public QsiColumnsDeclarationNode DynamicColumns { get; set; }
    }
}

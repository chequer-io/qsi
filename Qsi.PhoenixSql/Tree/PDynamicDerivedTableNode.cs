using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal sealed class PDynamicDerivedTableNode : QsiDerivedTableNode, IDynamicTableNode
    {
        public QsiColumnsDeclarationNode DynamicColumns { get; set; }
    }
}

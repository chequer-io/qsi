using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal interface IDynamicTableNode
    {
        QsiColumnsDeclarationNode DynamicColumns { get; }
    }
}

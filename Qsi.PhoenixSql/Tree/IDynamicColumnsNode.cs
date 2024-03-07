using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree;

internal interface IDynamicColumnsNode
{
    QsiColumnsDeclarationNode DynamicColumns { get; }
}
using Qsi.Tree;
using Qsi.Tree.Definition;

namespace Qsi.PhoenixSql.Tree;

public sealed class PViewDefinitionNode : QsiViewDefinitionNode, IDynamicColumnsNode
{
    public QsiColumnsDeclarationNode DynamicColumns { get; set; }
}
using Qsi.Tree;
using Qsi.Tree.Definition;

namespace Qsi.SingleStore.Tree;

public sealed class SingleStoreViewDefinitionNode : QsiViewDefinitionNode
{
    public QsiTreeNodeProperty<QsiExpressionFragmentNode> ViewAlgorithm { get; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> ViewSuid { get; }

    public string Definer { get; set; }

    public bool Replace { get; set; }

    public SingleStoreViewDefinitionNode()
    {
        ViewAlgorithm = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        ViewSuid = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
    }
}

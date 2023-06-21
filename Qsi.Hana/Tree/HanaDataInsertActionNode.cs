using Qsi.Tree;

namespace Qsi.Hana.Tree;

public sealed class HanaDataInsertActionNode : QsiDataInsertActionNode
{
    public QsiTreeNodeProperty<QsiExpressionFragmentNode> Overriding { get; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> Hint { get; }

    public HanaDataInsertActionNode()
    {
        Overriding = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        Hint = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
    }
}
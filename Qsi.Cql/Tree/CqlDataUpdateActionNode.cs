using Qsi.Tree;

namespace Qsi.Cql.Tree;

public sealed class CqlDataUpdateActionNode : QsiDataUpdateActionNode
{
    public QsiTreeNodeProperty<CqlMultipleUsingExpressionNode> Usings { get; }

    public CqlDataUpdateActionNode()
    {
        Usings = new QsiTreeNodeProperty<CqlMultipleUsingExpressionNode>(this);
    }
}
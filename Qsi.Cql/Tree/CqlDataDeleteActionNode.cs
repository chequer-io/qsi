using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDataDeleteActionNode : QsiDataDeleteActionNode
    {
        public QsiTreeNodeProperty<CqlUsingExpressionNode> Using { get; }

        public CqlDataDeleteActionNode()
        {
            Using = new QsiTreeNodeProperty<CqlUsingExpressionNode>(this);
        }
    }
}

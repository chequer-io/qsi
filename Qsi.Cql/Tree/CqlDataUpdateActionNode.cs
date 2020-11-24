using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDataUpdateActionNode : QsiDataUpdateActionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Using { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> StaticColumnCondition { get; }

        public CqlDataUpdateActionNode()
        {
            Using = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            StaticColumnCondition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDataDeleteActionNode : QsiDataDeleteActionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> StaticColumnCondition { get; }

        public QsiTreeNodeProperty<CqlUsingExpressionNode> Using { get; }

        public CqlDataDeleteActionNode()
        {
            StaticColumnCondition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Using = new QsiTreeNodeProperty<CqlUsingExpressionNode>(this);
        }
    }
}

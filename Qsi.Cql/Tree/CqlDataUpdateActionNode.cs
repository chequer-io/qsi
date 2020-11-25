using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDataUpdateActionNode : QsiDataUpdateActionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> StaticColumnCondition { get; }

        public QsiTreeNodeProperty<CqlMultipleUsingExpressionNode> Usings { get; }

        public CqlDataUpdateActionNode()
        {
            StaticColumnCondition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Usings = new QsiTreeNodeProperty<CqlMultipleUsingExpressionNode>(this);
        }
    }
}

using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDataDeleteActionNode : QsiDataDeleteActionNode
    {
        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> TargetColumns { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> StaticColumnCondition { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Using { get; }

        public CqlDataDeleteActionNode()
        {
            TargetColumns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
            StaticColumnCondition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Using = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

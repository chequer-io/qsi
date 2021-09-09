using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public class TrinoJoinedTableNode : QsiJoinedTableNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> OnCondition { get; }

        public TrinoJoinedTableNode()
        {
            OnCondition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

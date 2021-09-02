using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleJoinedTableNode : QsiJoinedTableNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> OnCondition { get; }

        public OracleJoinedTableNode()
        {
            OnCondition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

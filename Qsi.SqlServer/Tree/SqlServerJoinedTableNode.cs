using Qsi.Tree;

namespace Qsi.SqlServer.Tree
{
    public class SqlServerJoinedTableNode : QsiJoinedTableNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public SqlServerJoinedTableNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

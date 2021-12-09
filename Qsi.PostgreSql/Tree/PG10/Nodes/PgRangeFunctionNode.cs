using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.PG10.Nodes
{
    public class PgRangeFunctionNode : QsiInlineDerivedTableNode
    {
        public QsiTreeNodeProperty<QsiInvokeExpressionNode> Source { get; }

        public PgRangeFunctionNode()
        {
            Source = new QsiTreeNodeProperty<QsiInvokeExpressionNode>(this);
        }
    }
}

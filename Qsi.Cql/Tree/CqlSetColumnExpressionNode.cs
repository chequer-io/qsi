using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlSetColumnExpressionNode : QsiSetColumnExpressionNode
    {
        public QsiTreeNodeProperty<QsiMemberAccessExpressionNode> TargetExpression { get; }

        public CqlSetOperator Operator { get; set; }

        public CqlSetColumnExpressionNode()
        {
            TargetExpression = new QsiTreeNodeProperty<QsiMemberAccessExpressionNode>(this);
        }
    }
}

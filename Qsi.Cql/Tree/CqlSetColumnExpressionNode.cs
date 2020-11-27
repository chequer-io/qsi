using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlSetColumnExpressionNode : QsiSetColumnExpressionNode
    {
        public QsiTreeNodeProperty<QsiMemberAccessExpressionNode> TargetExpression { get; }

        public CqlSetOperator Operator { get; set; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!TargetExpression.IsEmpty)
                    yield return TargetExpression.Value;

                if (!Value.IsEmpty)
                    yield return Value.Value;
            }
        }

        public CqlSetColumnExpressionNode()
        {
            TargetExpression = new QsiTreeNodeProperty<QsiMemberAccessExpressionNode>(this);
        }
    }
}

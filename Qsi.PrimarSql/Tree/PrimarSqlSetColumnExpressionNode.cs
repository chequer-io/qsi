using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PrimarSql.Tree
{
    public sealed class PrimarSqlSetColumnExpressionNode : QsiSetColumnExpressionNode
    {
        public QsiTreeNodeProperty<QsiMemberAccessExpressionNode> TargetExpression { get; }

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

        public PrimarSqlSetColumnExpressionNode()
        {
            TargetExpression = new QsiTreeNodeProperty<QsiMemberAccessExpressionNode>(this);
        }
    }
}

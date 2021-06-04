using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaAssociationTableNode : QsiTableNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Condition { get; }

        public QsiTreeNodeProperty<HanaAssociationExpressionNode> Expression { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Condition.IsEmpty)
                    yield return Condition.Value;

                if (!Expression.IsEmpty)
                    yield return Expression.Value;
            }
        }

        public HanaAssociationTableNode()
        {
            Condition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Expression = new QsiTreeNodeProperty<HanaAssociationExpressionNode>(this);
        }
    }
}

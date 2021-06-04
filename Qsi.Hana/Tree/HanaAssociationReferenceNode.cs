using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaAssociationReferenceNode : QsiExpressionNode
    {
        public QsiIdentifier Identifier { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Condition { get; }

        public string Cardinality { get; set; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Condition.IsEmpty)
                    yield return Condition.Value;
            }
        }

        public HanaAssociationReferenceNode()
        {
            Condition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaTableReferenceNode : QsiTableReferenceNode
    {
        public QsiTreeNodeProperty<HanaTableBehaviorNode> Behavior { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Partition { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                foreach (var child in base.Children)
                    yield return child;

                if (!Behavior.IsEmpty)
                    yield return Behavior.Value;

                if (!Partition.IsEmpty)
                    yield return Partition.Value;
            }
        }

        public HanaTableReferenceNode()
        {
            Behavior = new QsiTreeNodeProperty<HanaTableBehaviorNode>(this);
            Partition = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }
}

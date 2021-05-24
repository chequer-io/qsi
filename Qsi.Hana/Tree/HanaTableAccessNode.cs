using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaTableAccessNode : QsiTableAccessNode
    {
        public QsiTreeNodeProperty<HanaTableBehaviorNode> Behavior { get; }

        // PARTITION (1, 2, ..)
        public string Partition { get; set; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                foreach (var child in base.Children)
                    yield return child;

                if (!Behavior.IsEmpty)
                    yield return Behavior.Value;
            }
        }

        public HanaTableAccessNode()
        {
            Behavior = new QsiTreeNodeProperty<HanaTableBehaviorNode>(this);
        }
    }
}

using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaDerivedTableNode : QsiDerivedTableNode
    {
        public QsiTreeNodeProperty<HanaTableBehaviorNode> Behavior { get; }

        public string TimeTravel { get; set; }

        public string Hint { get; set; }

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

        public HanaDerivedTableNode()
        {
            Behavior = new QsiTreeNodeProperty<HanaTableBehaviorNode>(this);
        }
    }
}

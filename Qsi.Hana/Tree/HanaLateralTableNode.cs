using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaLateralTableNode : QsiTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Source { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Source.IsEmpty)
                    yield return Source.Value;
            }
        }

        public HanaLateralTableNode()
        {
            Source = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}

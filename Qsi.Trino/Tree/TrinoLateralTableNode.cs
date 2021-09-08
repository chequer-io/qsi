using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public sealed class TrinoLateralTableNode : QsiTableNode
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

        public TrinoLateralTableNode()
        {
            Source = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}

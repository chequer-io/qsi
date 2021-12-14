using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree
{
    public sealed class AthenaLateralTableNode : QsiTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Source { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Source);

        public AthenaLateralTableNode()
        {
            Source = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}

using System.Collections.Generic;
using Qsi.Athena.Tree.Nodes;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Common
{
    public class AthenaDerivedTableNode : QsiDerivedTableNode
    {
        public AthenaSetQuantifier? SetQuantifier { get; set; }

        public QsiTreeNodeProperty<AthenaWindowExpressionNode> Window { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(base.Children, Window);

        public AthenaDerivedTableNode()
        {
            Window = new QsiTreeNodeProperty<AthenaWindowExpressionNode>(this);
        }
    }
}

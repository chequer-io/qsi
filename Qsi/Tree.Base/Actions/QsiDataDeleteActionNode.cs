using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiDataDeleteActionNode : QsiActionNode, IQsiDataDeleteActionNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Target { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target);

        #region Explicit
        IQsiTableNode IQsiDataDeleteActionNode.Target => Target.Value;
        #endregion

        public QsiDataDeleteActionNode()
        {
            Target = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}

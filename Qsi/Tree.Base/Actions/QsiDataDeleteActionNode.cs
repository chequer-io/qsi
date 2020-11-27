using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiDataDeleteActionNode : QsiActionNode, IQsiDataDeleteActionNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Target { get; }

        public QsiQualifiedIdentifier[] Columns { get; set; }

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

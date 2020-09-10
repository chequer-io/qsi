using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public abstract class QsiTreeNode : IQsiTreeNode
    {
        public QsiTreeNode Parent { get; set; }

        public abstract IEnumerable<IQsiTreeNode> Children { get; }

        #region Explicit
        IQsiTreeNode IQsiTreeNode.Parent => Parent;
        #endregion
    }
}

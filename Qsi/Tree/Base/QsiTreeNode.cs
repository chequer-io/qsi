using System;

namespace Qsi.Tree.Base
{
    public abstract class QsiTreeNode : IQsiTreeNode
    {
        public QsiTreeNode Parent { get; set; }

        #region Explicit
        IQsiTreeNode IQsiTreeNode.Parent => Parent;
        #endregion
    }
}

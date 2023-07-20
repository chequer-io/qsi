using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree;

public abstract class QsiTreeNode : IQsiTreeNode
{
    public QsiTreeNode Parent { get; set; }

    public abstract IEnumerable<IQsiTreeNode> Children { get; }

    public IUserDataHolder UserData => _userData ??= new UserDataHolder();

    #region Explicit
    IQsiTreeNode IQsiTreeNode.Parent => Parent;
    #endregion

    private IUserDataHolder _userData;
}
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableDataDeleteActionNode : IQsiDataDeleteActionNode
{
    public IQsiTreeNode Parent { get; }

    public IUserDataHolder UserData { get; }

    public IQsiTableNode Target { get; }

    public QsiQualifiedIdentifier[] Columns { get; }

    public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target);

    public ImmutableDataDeleteActionNode(
        IQsiTreeNode parent,
        IQsiTableNode target,
        QsiQualifiedIdentifier[] columns,
        IUserDataHolder userData)
    {
        Parent = parent;
        Target = target;
        Columns = columns;
        UserData = userData;
    }
}
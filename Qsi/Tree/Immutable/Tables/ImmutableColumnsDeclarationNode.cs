using System.Collections;
using System.Collections.Generic;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableColumnsDeclarationNode : IQsiColumnsDeclarationNode
{
    public IQsiTreeNode Parent { get; }

    public int Count { get; }

    public IQsiColumnNode[] Columns { get; }

    public bool IsEmpty { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => Columns;

    public ImmutableColumnsDeclarationNode(IQsiTreeNode parent, IQsiColumnNode[] columns, IUserDataHolder userData)
    {
        Parent = parent;
        Count = columns?.Length ?? 0;
        Columns = columns;
        IsEmpty = Count == 0;
        UserData = userData;
    }

    public IEnumerator<IQsiColumnNode> GetEnumerator()
    {
        return ((IList<IQsiColumnNode>)Columns).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
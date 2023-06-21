using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable;

public readonly struct ImmutableAllColumnNode : IQsiAllColumnNode, IQsiTerminalNode
{
    public IQsiTreeNode Parent { get; }

    public QsiQualifiedIdentifier Path { get; }

    public bool IncludeInvisibleColumns { get; }

    public IQsiSequentialColumnNode[] SequentialColumns { get; }

    public IUserDataHolder UserData { get; }

    public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

    public ImmutableAllColumnNode(IQsiTreeNode parent, QsiQualifiedIdentifier path, IQsiSequentialColumnNode[] sequentialColumns, IUserDataHolder userData)
    {
        Parent = parent;
        Path = path;
        SequentialColumns = sequentialColumns;
        UserData = userData;
        IncludeInvisibleColumns = false;
    }
}
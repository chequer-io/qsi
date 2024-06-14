using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.SingleStore.Data;
using Qsi.Tree;

namespace Qsi.SingleStore.Tree;

public sealed class SingleStoreLockingNode : QsiTreeNode
{
    public SingleStoreTableLockType TableLockType { get; set; }

    public SingleStoreRowLockType? RowLockType { get; set; }

    public QsiQualifiedIdentifier[] Tables { get; set; }

    public override IEnumerable<IQsiTreeNode> Children { get; } = Enumerable.Empty<IQsiTreeNode>();
}

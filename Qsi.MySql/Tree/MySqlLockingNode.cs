using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.MySql.Data;
using Qsi.Tree;

namespace Qsi.MySql.Tree;

public sealed class MySqlLockingNode : QsiTreeNode
{
    public MySqlTableLockType TableLockType { get; set; }

    public MySqlRowLockType? RowLockType { get; set; }

    public QsiQualifiedIdentifier[] Tables { get; set; }

    public override IEnumerable<IQsiTreeNode> Children { get; } = Enumerable.Empty<IQsiTreeNode>();
}
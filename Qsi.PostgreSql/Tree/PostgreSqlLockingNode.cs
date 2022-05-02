using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public sealed class PostgreSqlLockingNode : QsiTreeNode
{
    public PostgreSqlTableLockType TableLockType { get; set; }
    
    public PostgreSqlRowLockType? RowLockType { get; set; }
    
    public QsiQualifiedIdentifier[] Tables { get; set; }

    public override IEnumerable<IQsiTreeNode> Children { get; } = Enumerable.Empty<IQsiTreeNode>();
}

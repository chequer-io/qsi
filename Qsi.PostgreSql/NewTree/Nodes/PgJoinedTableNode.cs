using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgJoinedTableNode : QsiJoinedTableNode
{
    public QsiIdentifier? JoinUsingAlias { get; set; }
}

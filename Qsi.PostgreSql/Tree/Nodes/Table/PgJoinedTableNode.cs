using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgJoinedTableNode : QsiJoinedTableNode
{
    public QsiIdentifier? JoinUsingAlias { get; set; }
}

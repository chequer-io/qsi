using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgRowValueExpressionNode : QsiRowValueExpressionNode
{
    // Explicit: ROW (1,2)
    // Implicit: (1,2)
    public bool IsExplicit { get; set; }
}

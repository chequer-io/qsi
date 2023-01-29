using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgLimitExpressionNode : QsiLimitExpressionNode
{
    public LimitOption Option { get; set; }
}

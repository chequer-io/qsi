using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgLimitExpressionNode : QsiLimitExpressionNode
{
    public LimitOption Option { get; set; }
}

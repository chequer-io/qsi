using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgBooleanMultipleExpressionNode : QsiMultipleExpressionNode
{
    public BoolExprType Type { get; set; } 
}

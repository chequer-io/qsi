using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgBooleanMultipleExpressionNode : QsiMultipleExpressionNode
{
    public BoolExprType Type { get; set; } 
}

using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgUnaryExpressionNode : QsiUnaryExpressionNode
{
    public AExprKind ExprKind { get; set; }
    
    public BoolExprType BoolKind { get; set; }
}

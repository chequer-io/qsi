using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgUnaryExpressionNode : QsiUnaryExpressionNode
{
    public A_Expr_Kind ExprKind { get; set; }
    
    public BoolExprType BoolKind { get; set; }
}

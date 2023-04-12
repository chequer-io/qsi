using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgBinaryExpressionNode : QsiBinaryExpressionNode
{
    public A_Expr_Kind ExprKind { get; set; } = A_Expr_Kind.Undefined;
}

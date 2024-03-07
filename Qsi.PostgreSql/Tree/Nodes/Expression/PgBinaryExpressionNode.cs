using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgBinaryExpressionNode : QsiBinaryExpressionNode
{
    public AExprKind ExprKind { get; set; } = AExprKind.AExprKindUndefined;
}

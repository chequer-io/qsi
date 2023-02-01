using PgQuery;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgSqlValueInvokeExpressionNode : PgInvokeExpressionNode
{
    public SQLValueFunctionOp FunctionOp { get; set; }
}

using PgQuery;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgSqlValueInvokeExpressionNode : PgInvokeExpressionNode
{
    public SQLValueFunctionOp FunctionOp { get; set; }
}

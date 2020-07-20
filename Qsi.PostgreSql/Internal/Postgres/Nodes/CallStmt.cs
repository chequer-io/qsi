// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CallStmt")]
    internal class CallStmt : Node
    {
        public FuncCall funccall { get; set; }

        public FuncExpr funcexpr { get; set; }
    }
}

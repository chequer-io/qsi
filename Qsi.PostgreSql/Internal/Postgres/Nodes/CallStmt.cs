// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CallStmt")]
    internal class CallStmt : IPgTree
    {
        public FuncCall funccall { get; set; }

        public FuncExpr funcexpr { get; set; }
    }
}

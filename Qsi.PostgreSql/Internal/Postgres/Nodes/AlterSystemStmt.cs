// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterSystemStmt")]
    internal class AlterSystemStmt : IPgTree
    {
        public VariableSetStmt setstmt { get; set; }
    }
}

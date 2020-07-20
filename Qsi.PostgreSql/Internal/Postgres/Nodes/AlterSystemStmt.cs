// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterSystemStmt")]
    internal class AlterSystemStmt : Node
    {
        public VariableSetStmt setstmt { get; set; }
    }
}

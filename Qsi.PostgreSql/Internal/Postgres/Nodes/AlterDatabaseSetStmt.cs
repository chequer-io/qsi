// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterDatabaseSetStmt")]
    internal class AlterDatabaseSetStmt : Node
    {
        public char dbname { get; set; }

        public VariableSetStmt setstmt { get; set; }
    }
}

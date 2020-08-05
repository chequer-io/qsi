// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterDatabaseSetStmt")]
    internal class AlterDatabaseSetStmt : IPgTree
    {
        public string dbname { get; set; }

        public VariableSetStmt setstmt { get; set; }
    }
}

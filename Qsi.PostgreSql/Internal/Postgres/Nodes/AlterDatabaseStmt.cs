// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterDatabaseStmt")]
    internal class AlterDatabaseStmt : Node
    {
        public char dbname { get; set; }

        public IPgTree[] options { get; set; }
    }
}

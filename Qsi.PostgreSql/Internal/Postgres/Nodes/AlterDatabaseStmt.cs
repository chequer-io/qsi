// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterDatabaseStmt")]
    internal class AlterDatabaseStmt : IPgTree
    {
        public string dbname { get; set; }

        public IPgTree[] options { get; set; }
    }
}

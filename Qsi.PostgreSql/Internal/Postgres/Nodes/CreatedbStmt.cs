// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreatedbStmt")]
    internal class CreatedbStmt : IPgTree
    {
        public string dbname { get; set; }

        public IPgTree[] options { get; set; }
    }
}

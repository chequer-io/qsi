// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreatedbStmt")]
    internal class CreatedbStmt : Node
    {
        public char dbname { get; set; }

        public IPgTree[] options { get; set; }
    }
}

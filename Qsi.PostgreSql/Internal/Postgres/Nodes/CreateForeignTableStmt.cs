// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class CreateForeignTableStmt
    {
        public CreateStmt @base { get; set; }

        public string servername { get; set; }

        public IPgTree[] options { get; set; }
    }
}

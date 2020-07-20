// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreatePublicationStmt")]
    internal class CreatePublicationStmt : Node
    {
        public char pubname { get; set; }

        public IPgTree[] options { get; set; }

        public IPgTree[] tables { get; set; }

        public bool for_all_tables { get; set; }
    }
}

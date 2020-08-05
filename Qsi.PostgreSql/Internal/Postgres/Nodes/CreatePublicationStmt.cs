// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreatePublicationStmt")]
    internal class CreatePublicationStmt : IPgTree
    {
        public string pubname { get; set; }

        public IPgTree[] options { get; set; }

        public IPgTree[] tables { get; set; }

        public bool for_all_tables { get; set; }
    }
}

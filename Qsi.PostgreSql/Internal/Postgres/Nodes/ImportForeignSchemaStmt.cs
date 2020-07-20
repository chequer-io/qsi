// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ImportForeignSchemaStmt")]
    internal class ImportForeignSchemaStmt : Node
    {
        public char server_name { get; set; }

        public char remote_schema { get; set; }

        public char local_schema { get; set; }

        public ImportForeignSchemaType list_type { get; set; }

        public IPgTree[] table_list { get; set; }

        public IPgTree[] options { get; set; }
    }
}

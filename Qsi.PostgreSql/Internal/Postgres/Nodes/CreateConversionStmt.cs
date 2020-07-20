// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateConversionStmt")]
    internal class CreateConversionStmt : Node
    {
        public IPgTree[] conversion_name { get; set; }

        public string for_encoding_name { get; set; }

        public string to_encoding_name { get; set; }

        public IPgTree[] func_name { get; set; }

        public bool def { get; set; }
    }
}

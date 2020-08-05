// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateConversionStmt")]
    internal class CreateConversionStmt : IPgTree
    {
        public IPgTree[] conversion_name { get; set; }

        public string for_encoding_name { get; set; }

        public string to_encoding_name { get; set; }

        public IPgTree[] func_name { get; set; }

        public bool def { get; set; }
    }
}

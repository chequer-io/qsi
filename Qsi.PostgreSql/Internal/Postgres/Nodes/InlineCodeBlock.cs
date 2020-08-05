// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("InlineCodeBlock")]
    internal class InlineCodeBlock : IPgTree
    {
        public string source_text { get; set; }

        public int /* oid */ langOid { get; set; }

        public bool langIsTrusted { get; set; }

        public bool atomic { get; set; }
    }
}

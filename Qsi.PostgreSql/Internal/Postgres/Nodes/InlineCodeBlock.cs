// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("InlineCodeBlock")]
    internal class InlineCodeBlock : Node
    {
        public char source_text { get; set; }

        public string /* oid */ langOid { get; set; }

        public bool langIsTrusted { get; set; }

        public bool atomic { get; set; }
    }
}

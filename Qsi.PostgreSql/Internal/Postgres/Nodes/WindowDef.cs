// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("WindowDef")]
    internal class WindowDef : Node
    {
        public string name { get; set; }

        public string refname { get; set; }

        public IPgTree[] partitionClause { get; set; }

        public IPgTree[] orderClause { get; set; }

        public int frameOptions { get; set; }

        public Node startOffset { get; set; }

        public Node endOffset { get; set; }

        public int location { get; set; }
    }
}

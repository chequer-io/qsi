// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("WindowDef")]
    internal class WindowDef : IPgTree
    {
        public string name { get; set; }

        public string refname { get; set; }

        public IPgTree[] partitionClause { get; set; }

        public IPgTree[] orderClause { get; set; }

        public int frameOptions { get; set; }

        public IPgTree startOffset { get; set; }

        public IPgTree endOffset { get; set; }

        public int location { get; set; }
    }
}

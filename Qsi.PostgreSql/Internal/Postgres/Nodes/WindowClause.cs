// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("WindowClause")]
    internal class WindowClause : IPgTree
    {
        public string name { get; set; }

        public string refname { get; set; }

        public IPgTree[] partitionClause { get; set; }

        public IPgTree[] orderClause { get; set; }

        public int frameOptions { get; set; }

        public IPgTree startOffset { get; set; }

        public IPgTree endOffset { get; set; }

        public int /* oid */ startInRangeFunc { get; set; }

        public int /* oid */ endInRangeFunc { get; set; }

        public int /* oid */ inRangeColl { get; set; }

        public bool inRangeAsc { get; set; }

        public bool inRangeNullsFirst { get; set; }

        public index winref { get; set; }

        public bool copiedOrder { get; set; }
    }
}

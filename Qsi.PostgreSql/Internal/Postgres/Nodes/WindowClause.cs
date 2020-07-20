// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("WindowClause")]
    internal class WindowClause : Node
    {
        public char name { get; set; }

        public char refname { get; set; }

        public IPgTree[] partitionClause { get; set; }

        public IPgTree[] orderClause { get; set; }

        public int frameOptions { get; set; }

        public Node startOffset { get; set; }

        public Node endOffset { get; set; }

        public string /* oid */ startInRangeFunc { get; set; }

        public string /* oid */ endInRangeFunc { get; set; }

        public string /* oid */ inRangeColl { get; set; }

        public bool inRangeAsc { get; set; }

        public bool inRangeNullsFirst { get; set; }

        public index winref { get; set; }

        public bool copiedOrder { get; set; }
    }
}

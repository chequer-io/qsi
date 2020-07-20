// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("SortGroupClause")]
    internal class SortGroupClause : Node
    {
        public index tleSortGroupRef { get; set; }

        public string /* oid */ eqop { get; set; }

        public string /* oid */ sortop { get; set; }

        public bool nulls_first { get; set; }

        public bool hashable { get; set; }
    }
}

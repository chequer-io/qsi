// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("IndexElem")]
    internal class IndexElem : Node
    {
        public string name { get; set; }

        public Node expr { get; set; }

        public string indexcolname { get; set; }

        public IPgTree[] collation { get; set; }

        public IPgTree[] opclass { get; set; }

        public IPgTree[] opclassopts { get; set; }

        public SortByDir ordering { get; set; }

        public SortByNulls nulls_ordering { get; set; }
    }
}

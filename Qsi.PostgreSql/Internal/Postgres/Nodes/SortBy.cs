// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("SortBy")]
    internal class SortBy : IPgTree
    {
        public IPgTree node { get; set; }

        public SortByDir sortby_dir { get; set; }

        public SortByNulls sortby_nulls { get; set; }

        public IPgTree[] useOp { get; set; }

        public int location { get; set; }
    }
}

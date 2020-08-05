// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("MultiAssignRef")]
    internal class MultiAssignRef : IPgTree
    {
        public IPgTree source { get; set; }

        public int colno { get; set; }

        public int ncolumns { get; set; }
    }
}

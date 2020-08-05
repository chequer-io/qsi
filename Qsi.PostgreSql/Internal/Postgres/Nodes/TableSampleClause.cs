// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("TableSampleClause")]
    internal class TableSampleClause : IPgTree
    {
        public int /* oid */ tsmhandler { get; set; }

        public IPgTree[] args { get; set; }

        public Expr repeatable { get; set; }
    }
}

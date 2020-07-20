// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("TableSampleClause")]
    internal class TableSampleClause : Node
    {
        public int /* oid */ tsmhandler { get; set; }

        public IPgTree[] args { get; set; }

        public Expr repeatable { get; set; }
    }
}

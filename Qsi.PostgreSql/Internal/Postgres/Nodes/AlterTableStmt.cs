// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterTableStmt")]
    internal class AlterTableStmt : IPgTree
    {
        public RangeVar relation { get; set; }

        public IPgTree[] cmds { get; set; }

        public ObjectType objtype { get; set; }

        public bool missing_ok { get; set; }
    }
}

// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("OnConflictExpr")]
    internal class OnConflictExpr : IPgTree
    {
        public OnConflictAction action { get; set; }

        public IPgTree[] arbiterElems { get; set; }

        public IPgTree arbiterWhere { get; set; }

        public int /* oid */ constraint { get; set; }

        public IPgTree[] onConflictSet { get; set; }

        public IPgTree onConflictWhere { get; set; }

        public int exclRelIndex { get; set; }

        public IPgTree[] exclRelTlist { get; set; }
    }
}

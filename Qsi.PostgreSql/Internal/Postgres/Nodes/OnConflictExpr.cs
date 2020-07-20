// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("OnConflictExpr")]
    internal class OnConflictExpr : Node
    {
        public OnConflictAction action { get; set; }

        public IPgTree[] arbiterElems { get; set; }

        public Node arbiterWhere { get; set; }

        public int /* oid */ constraint { get; set; }

        public IPgTree[] onConflictSet { get; set; }

        public Node onConflictWhere { get; set; }

        public int exclRelIndex { get; set; }

        public IPgTree[] exclRelTlist { get; set; }
    }
}

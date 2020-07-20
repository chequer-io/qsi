// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class TargetEntry
    {
        public Expr xpr { get; set; }

        public Expr expr { get; set; }

        public short /* AttrNumber */ resno { get; set; }

        public char resname { get; set; }

        public index ressortgroupref { get; set; }

        public string /* oid */ resorigtbl { get; set; }

        public short /* AttrNumber */ resorigcol { get; set; }

        public bool resjunk { get; set; }
    }
}

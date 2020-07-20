// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class OpExpr
    {
        public Expr xpr { get; set; }

        public int /* oid */ opno { get; set; }

        public int /* oid */ opfuncid { get; set; }

        public int /* oid */ opresulttype { get; set; }

        public bool opretset { get; set; }

        public int /* oid */ opcollid { get; set; }

        public int /* oid */ inputcollid { get; set; }

        public IPgTree[] args { get; set; }

        public int location { get; set; }
    }
}

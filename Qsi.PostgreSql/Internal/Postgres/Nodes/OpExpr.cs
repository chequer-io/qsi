// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class OpExpr
    {
        public Expr xpr { get; set; }

        public string /* oid */ opno { get; set; }

        public string /* oid */ opfuncid { get; set; }

        public string /* oid */ opresulttype { get; set; }

        public bool opretset { get; set; }

        public string /* oid */ opcollid { get; set; }

        public string /* oid */ inputcollid { get; set; }

        public IPgTree[] args { get; set; }

        public int location { get; set; }
    }
}

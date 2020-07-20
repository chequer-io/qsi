// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class Param
    {
        public Expr xpr { get; set; }

        public ParamKind paramkind { get; set; }

        public int paramid { get; set; }

        public string /* oid */ paramtype { get; set; }

        public int paramtypmod { get; set; }

        public string /* oid */ paramcollid { get; set; }

        public int location { get; set; }
    }
}

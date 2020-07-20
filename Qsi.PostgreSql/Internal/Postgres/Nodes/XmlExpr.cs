// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class XmlExpr
    {
        public Expr xpr { get; set; }

        public XmlExprOp op { get; set; }

        public char name { get; set; }

        public IPgTree[] named_args { get; set; }

        public IPgTree[] arg_names { get; set; }

        public IPgTree[] args { get; set; }

        public XmlOptionType xmloption { get; set; }

        public string /* oid */ type { get; set; }

        public int typmod { get; set; }

        public int location { get; set; }
    }
}

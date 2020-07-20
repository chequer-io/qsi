// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class InferenceElem
    {
        public Expr xpr { get; set; }

        public Node expr { get; set; }

        public int /* oid */ infercollid { get; set; }

        public int /* oid */ inferopclass { get; set; }
    }
}

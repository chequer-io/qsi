// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class InferenceElem
    {
        public Expr xpr { get; set; }

        public Node expr { get; set; }

        public string /* oid */ infercollid { get; set; }

        public string /* oid */ inferopclass { get; set; }
    }
}

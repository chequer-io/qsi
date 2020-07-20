// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class NullTest
    {
        public Expr xpr { get; set; }

        public Expr arg { get; set; }

        public NullTestType nulltesttype { get; set; }

        public bool argisrow { get; set; }

        public int location { get; set; }
    }
}

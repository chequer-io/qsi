// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class SQLValueFunction
    {
        public Expr xpr { get; set; }

        public SQLValueFunctionOp op { get; set; }

        public int /* oid */ type { get; set; }

        public int typmod { get; set; }

        public int location { get; set; }
    }
}

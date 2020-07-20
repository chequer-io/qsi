// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class AlternativeSubPlan
    {
        public Expr xpr { get; set; }

        public IPgTree[] subplans { get; set; }
    }
}

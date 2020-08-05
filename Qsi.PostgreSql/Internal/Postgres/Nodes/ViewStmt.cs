// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ViewStmt")]
    internal class ViewStmt : IPgTree
    {
        public RangeVar view { get; set; }

        public IPgTree[] aliases { get; set; }

        public IPgTree query { get; set; }

        public bool replace { get; set; }

        public IPgTree[] options { get; set; }

        public ViewCheckOption withCheckOption { get; set; }
    }
}

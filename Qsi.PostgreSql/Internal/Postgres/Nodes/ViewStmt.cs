// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ViewStmt")]
    internal class ViewStmt : Node
    {
        public RangeVar view { get; set; }

        public IPgTree[] aliases { get; set; }

        public Node query { get; set; }

        public bool replace { get; set; }

        public IPgTree[] options { get; set; }

        public ViewCheckOption withCheckOption { get; set; }
    }
}

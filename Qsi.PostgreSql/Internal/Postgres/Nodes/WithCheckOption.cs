// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("WithCheckOption")]
    internal class WithCheckOption : Node
    {
        public WCOKind kind { get; set; }

        public char relname { get; set; }

        public char polname { get; set; }

        public Node qual { get; set; }

        public bool cascaded { get; set; }
    }
}

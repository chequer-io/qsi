// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("WithCheckOption")]
    internal class WithCheckOption : IPgTree
    {
        public WCOKind kind { get; set; }

        public string relname { get; set; }

        public string polname { get; set; }

        public IPgTree qual { get; set; }

        public bool cascaded { get; set; }
    }
}

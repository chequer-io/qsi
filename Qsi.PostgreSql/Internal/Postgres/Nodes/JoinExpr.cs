// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("JoinExpr")]
    internal class JoinExpr : Node
    {
        public JoinType jointype { get; set; }

        public bool isNatural { get; set; }

        public Node larg { get; set; }

        public Node rarg { get; set; }

        public IPgTree[] usingClause { get; set; }

        public Node quals { get; set; }

        public Alias alias { get; set; }

        public int rtindex { get; set; }
    }
}

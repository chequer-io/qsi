// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("JoinExpr")]
    internal class JoinExpr : IPgTree
    {
        public JoinType jointype { get; set; }

        public bool isNatural { get; set; }

        public IPgTree larg { get; set; }

        public IPgTree rarg { get; set; }

        public IPgTree[] usingClause { get; set; }

        public IPgTree quals { get; set; }

        public Alias alias { get; set; }

        public int rtindex { get; set; }
    }
}

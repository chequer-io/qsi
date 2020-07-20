// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CommonTableExpr")]
    internal class CommonTableExpr : Node
    {
        public char ctename { get; set; }

        public IPgTree[] aliascolnames { get; set; }

        public CTEMaterialize ctematerialized { get; set; }

        public Node ctequery { get; set; }

        public int location { get; set; }

        public bool cterecursive { get; set; }

        public int cterefcount { get; set; }

        public IPgTree[] ctecolnames { get; set; }

        public IPgTree[] ctecoltypes { get; set; }

        public IPgTree[] ctecoltypmods { get; set; }

        public IPgTree[] ctecolcollations { get; set; }
    }
}

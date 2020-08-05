// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CommonTableExpr")]
    internal class CommonTableExpr : IPgTree
    {
        public string ctename { get; set; }

        public IPgTree[] aliascolnames { get; set; }

        public CTEMaterialize ctematerialized { get; set; }

        public IPgTree ctequery { get; set; }

        public int location { get; set; }

        public bool cterecursive { get; set; }

        public int cterefcount { get; set; }

        public IPgTree[] ctecolnames { get; set; }

        public IPgTree[] ctecoltypes { get; set; }

        public IPgTree[] ctecoltypmods { get; set; }

        public IPgTree[] ctecolcollations { get; set; }
    }
}

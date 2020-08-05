// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RangeTableFunc")]
    internal class RangeTableFunc : IPgTree
    {
        public bool lateral { get; set; }

        public IPgTree docexpr { get; set; }

        public IPgTree rowexpr { get; set; }

        public IPgTree[] namespaces { get; set; }

        public IPgTree[] columns { get; set; }

        public Alias alias { get; set; }

        public int location { get; set; }
    }
}

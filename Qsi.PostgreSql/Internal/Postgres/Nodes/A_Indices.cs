// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("A_Indices")]
    internal class A_Indices : IPgTree
    {
        public bool is_slice { get; set; }

        public IPgTree lidx { get; set; }

        public IPgTree uidx { get; set; }
    }
}

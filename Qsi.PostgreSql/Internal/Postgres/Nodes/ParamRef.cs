// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("ParamRef")]
    internal class ParamRef : IPgTree
    {
        public int number { get; set; }

        public int location { get; set; }
    }
}

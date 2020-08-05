// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("PrepareStmt")]
    internal class PrepareStmt : IPgTree
    {
        public string name { get; set; }

        public IPgTree[] argtypes { get; set; }

        public IPgTree query { get; set; }
    }
}

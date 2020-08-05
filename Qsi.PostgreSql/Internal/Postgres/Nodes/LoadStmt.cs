// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("LoadStmt")]
    internal class LoadStmt : IPgTree
    {
        public string filename { get; set; }
    }
}

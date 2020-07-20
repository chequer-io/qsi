// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("LoadStmt")]
    internal class LoadStmt : Node
    {
        public string filename { get; set; }
    }
}

// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateAmStmt")]
    internal class CreateAmStmt : Node
    {
        public string amname { get; set; }

        public IPgTree[] handler_name { get; set; }

        public char amtype { get; set; }
    }
}

// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateAmStmt")]
    internal class CreateAmStmt : IPgTree
    {
        public string amname { get; set; }

        public IPgTree[] handler_name { get; set; }

        public char amtype { get; set; }
    }
}

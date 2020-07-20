// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterFdwStmt")]
    internal class AlterFdwStmt : Node
    {
        public string fdwname { get; set; }

        public IPgTree[] func_options { get; set; }

        public IPgTree[] options { get; set; }
    }
}

// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateFdwStmt")]
    internal class CreateFdwStmt : Node
    {
        public char fdwname { get; set; }

        public IPgTree[] func_options { get; set; }

        public IPgTree[] options { get; set; }
    }
}

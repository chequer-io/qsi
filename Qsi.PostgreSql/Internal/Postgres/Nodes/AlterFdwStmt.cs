// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterFdwStmt")]
    internal class AlterFdwStmt : IPgTree
    {
        public string fdwname { get; set; }

        public IPgTree[] func_options { get; set; }

        public IPgTree[] options { get; set; }
    }
}

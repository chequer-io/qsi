// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("TransactionStmt")]
    internal class TransactionStmt : Node
    {
        public TransactionStmtKind kind { get; set; }

        public IPgTree[] options { get; set; }

        public string savepoint_name { get; set; }

        public string gid { get; set; }

        public bool chain { get; set; }
    }
}

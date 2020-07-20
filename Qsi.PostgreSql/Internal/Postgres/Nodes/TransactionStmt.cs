// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("TransactionStmt")]
    internal class TransactionStmt : Node
    {
        public TransactionStmtKind kind { get; set; }

        public IPgTree[] options { get; set; }

        public char savepoint_name { get; set; }

        public char gid { get; set; }

        public bool chain { get; set; }
    }
}

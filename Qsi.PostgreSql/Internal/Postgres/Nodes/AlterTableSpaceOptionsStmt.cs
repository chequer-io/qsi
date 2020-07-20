// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("AlterTableSpaceOptionsStmt")]
    internal class AlterTableSpaceOptionsStmt : Node
    {
        public char tablespacename { get; set; }

        public IPgTree[] options { get; set; }

        public bool isReset { get; set; }
    }
}

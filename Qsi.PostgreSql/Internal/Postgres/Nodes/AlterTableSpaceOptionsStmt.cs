// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("AlterTableSpaceOptionsStmt")]
    internal class AlterTableSpaceOptionsStmt : IPgTree
    {
        public string tablespacename { get; set; }

        public IPgTree[] options { get; set; }

        public bool isReset { get; set; }
    }
}

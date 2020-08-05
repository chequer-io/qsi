// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("SecLabelStmt")]
    internal class SecLabelStmt : IPgTree
    {
        public ObjectType objtype { get; set; }

        public IPgTree @object { get; set; }

        public string provider { get; set; }

        public string label { get; set; }
    }
}

// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateOpClassItem")]
    internal class CreateOpClassItem : IPgTree
    {
        public int itemtype { get; set; }

        public ObjectWithArgs name { get; set; }

        public int number { get; set; }

        public IPgTree[] order_family { get; set; }

        public IPgTree[] class_args { get; set; }

        public TypeName storedtype { get; set; }
    }
}

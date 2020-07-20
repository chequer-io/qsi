// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateOpClassItem")]
    internal class CreateOpClassItem : Node
    {
        public int itemtype { get; set; }

        public ObjectWithArgs name { get; set; }

        public int number { get; set; }

        public IPgTree[] order_family { get; set; }

        public IPgTree[] class_args { get; set; }

        public TypeName storedtype { get; set; }
    }
}

// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("TypeCast")]
    internal class TypeCast : Node
    {
        public Node arg { get; set; }

        public TypeName typeName { get; set; }

        public int location { get; set; }
    }
}

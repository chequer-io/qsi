// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("TypeCast")]
    internal class TypeCast : IPgTree
    {
        public IPgTree arg { get; set; }

        public TypeName typeName { get; set; }

        public int location { get; set; }
    }
}

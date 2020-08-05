// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateCastStmt")]
    internal class CreateCastStmt : IPgTree
    {
        public TypeName sourcetype { get; set; }

        public TypeName targettype { get; set; }

        public ObjectWithArgs func { get; set; }

        public CoercionContext context { get; set; }

        public bool inout { get; set; }
    }
}

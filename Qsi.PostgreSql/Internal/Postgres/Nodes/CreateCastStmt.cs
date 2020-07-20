// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateCastStmt")]
    internal class CreateCastStmt : Node
    {
        public TypeName sourcetype { get; set; }

        public TypeName targettype { get; set; }

        public ObjectWithArgs func { get; set; }

        public CoercionContext context { get; set; }

        public bool inout { get; set; }
    }
}

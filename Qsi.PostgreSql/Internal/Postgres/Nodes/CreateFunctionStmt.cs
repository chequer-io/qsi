// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateFunctionStmt")]
    internal class CreateFunctionStmt : Node
    {
        public bool is_procedure { get; set; }

        public bool replace { get; set; }

        public IPgTree[] funcname { get; set; }

        public IPgTree[] parameters { get; set; }

        public TypeName returnType { get; set; }

        public IPgTree[] options { get; set; }
    }
}

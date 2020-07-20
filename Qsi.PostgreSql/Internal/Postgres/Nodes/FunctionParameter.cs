// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("FunctionParameter")]
    internal class FunctionParameter : Node
    {
        public char name { get; set; }

        public TypeName argType { get; set; }

        public FunctionParameterMode mode { get; set; }

        public Node defexpr { get; set; }
    }
}

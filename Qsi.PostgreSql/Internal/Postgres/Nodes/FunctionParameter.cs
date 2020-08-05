// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("FunctionParameter")]
    internal class FunctionParameter : IPgTree
    {
        public string name { get; set; }

        public TypeName argType { get; set; }

        public FunctionParameterMode mode { get; set; }

        public IPgTree defexpr { get; set; }
    }
}

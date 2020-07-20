// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("ObjectWithArgs")]
    internal class ObjectWithArgs : Node
    {
        public IPgTree[] objname { get; set; }

        public IPgTree[] objargs { get; set; }

        public bool args_unspecified { get; set; }
    }
}

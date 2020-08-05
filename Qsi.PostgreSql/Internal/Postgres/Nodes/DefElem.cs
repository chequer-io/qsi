// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("DefElem")]
    internal class DefElem : IPgTree
    {
        public string defnamespace { get; set; }

        public string defname { get; set; }

        public IPgTree arg { get; set; }

        public DefElemAction defaction { get; set; }

        public int location { get; set; }
    }
}

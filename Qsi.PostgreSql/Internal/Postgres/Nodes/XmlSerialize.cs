// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("XmlSerialize")]
    internal class XmlSerialize : IPgTree
    {
        public XmlOptionType xmloption { get; set; }

        public IPgTree expr { get; set; }

        public TypeName typeName { get; set; }

        public int location { get; set; }
    }
}

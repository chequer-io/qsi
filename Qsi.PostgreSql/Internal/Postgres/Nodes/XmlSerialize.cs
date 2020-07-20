// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("XmlSerialize")]
    internal class XmlSerialize : Node
    {
        public XmlOptionType xmloption { get; set; }

        public Node expr { get; set; }

        public TypeName typeName { get; set; }

        public int location { get; set; }
    }
}

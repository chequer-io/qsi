// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeVar")]
    internal class RangeVar : Node
    {
        public char catalogname { get; set; }

        public char schemaname { get; set; }

        public char relname { get; set; }

        public bool inh { get; set; }

        public char relpersistence { get; set; }

        public Alias alias { get; set; }

        public int location { get; set; }
    }
}

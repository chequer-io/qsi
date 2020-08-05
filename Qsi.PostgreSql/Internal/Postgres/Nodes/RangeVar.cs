// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("RangeVar")]
    internal class RangeVar : IPgTree
    {
        public string catalogname { get; set; }

        public string schemaname { get; set; }

        public string relname { get; set; }

        public bool inh { get; set; }

        public char relpersistence { get; set; }

        public Alias alias { get; set; }

        public int location { get; set; }
    }
}

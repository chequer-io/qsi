// Generate from postgres/src/include/nodes/value.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("Value")]
    internal class PgValue : IPgTree
    {
        public int? ival { get; set; }

        public string str { get; set; }
    }

    [PgNode("String")]
    internal class PgString : PgValue
    {
    }

    [PgNode("Integer")]
    internal class PgInteger : PgValue
    {
    }

    [PgNode("Float")]
    internal class PgFloat : PgValue
    {
    }
}

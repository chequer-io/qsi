using Qsi.PostgreSql.Internal.PG10.Types;
using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal
{
    [PgNode("Integer")]
    internal sealed class PgInteger : Value
    {
        public override NodeTag Type { get; } = NodeTag.T_Integer;
    }

    [PgNode("Float")]
    internal sealed class PgFloat : Value
    {
        public override NodeTag Type { get; } = NodeTag.T_Float;
    }

    [PgNode("String")]
    internal sealed class PgString : Value
    {
        public override NodeTag Type { get; } = NodeTag.T_String;
    }

    [PgNode("BitString")]
    internal sealed class PgBitString : Value
    {
        public override NodeTag Type { get; } = NodeTag.T_BitString;
    }
}

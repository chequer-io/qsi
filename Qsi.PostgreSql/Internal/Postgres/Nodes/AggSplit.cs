// Generate from postgres/src/include/nodes/nodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum AggSplit
    {
        AGGSPLIT_SIMPLE = 0,
        /*
         * #define AGGSPLITOP_SKIPFINAL 0x02
         * #define AGGSPLITOP_SERIALIZE 0x04
        */
        AGGSPLIT_INITIAL_SERIAL = 0x02 | 0x04,
        /*
         * #define AGGSPLITOP_COMBINE 0x01
         * #define AGGSPLITOP_DESERIALIZE 0x08
        */
        AGGSPLIT_FINAL_DESERIAL = 0x01 | 0x08
    }
}

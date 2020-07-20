// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum PartitionRangeDatumKind
    {
        PARTITION_RANGE_DATUM_MINVALUE = -1,
        PARTITION_RANGE_DATUM_VALUE = 0,
        PARTITION_RANGE_DATUM_MAXVALUE = 1
    }
}

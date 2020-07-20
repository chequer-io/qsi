// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal enum QuerySource
    {
        QSRC_ORIGINAL,
        QSRC_PARSER,
        QSRC_INSTEAD_RULE,
        QSRC_QUAL_INSTEAD_RULE,
        QSRC_NON_INSTEAD_RULE
    }
}

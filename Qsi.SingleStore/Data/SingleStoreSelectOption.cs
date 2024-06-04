namespace Qsi.SingleStore.Data;

public enum SingleStoreSelectOption
{
    All,
    Distinct,
    StraightJoin,
    HighPriority,
    SqlSmallResult,
    SqlBigResult,
    SqlBufferResult,
    SqlCalcFoundRows,
    SqlNoCache,
    SqlCache,
    MaxStatementTime
}

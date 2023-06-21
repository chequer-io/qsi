namespace Qsi.MySql.Data;

public enum MySqlSelectOption
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
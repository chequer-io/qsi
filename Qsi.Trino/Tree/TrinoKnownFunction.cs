namespace Qsi.Trino.Tree;

public static class TrinoKnownFunction
{
    // GROUPING SETS ( <groupingSet>, .. )
    // ▶ GROUPING_SETS(<groupingSet>, ..)
    public const string GroupingSets = "GROUPING_SETS";

    // TRY_CAST ( expression AS type )
    // ▶ CAST ( expression AS type)
    public const string Cast = "CAST";

    // <expression> BETWEEN <expression> AND <expression>
    // ▶ BETWEEN(<expression>, <expression>, <expression>)
    public const string Between = "BETWEEN";

    // <expression> NOT BETWEEN <expression> AND <expression>
    // ▶ NOT_BETWEEN(<expression>, <expression>, <expression>)
    public const string NotBetween = "NOT_BETWEEN";

    // <expression> IN {<expression> | <subquery>}
    // ▶ IN(<expression>, {<expression> | <subquery>})
    public const string In = "IN";

    // <expression> NOT IN {<expression> | <subquery>}
    // ▶ NOT_IN(<expression>, {<expression> | <subquery>})
    public const string NotIn = "NOT_IN";

    // <expression> LIKE <expression> [ESCAPE <expression>]
    // ▶ LIKE(<expression>, <expression>[, <expression>])
    public const string Like = "LIKE";

    // <expression> NOT LIKE <expression> [ESCAPE <expression>]
    // ▶ NOT_LIKE(<expression>, <expression>[, <expression>])
    public const string NotLike = "NOT_LIKE";

    // <expression> IS NULL
    // ▶ IS_NULL(<expression>)
    public const string IsNull = "IS_NULL";

    // <expression> IS NOT NULL
    // ▶ IS_NOT_NULL(<expression>)
    public const string IsNotNull = "IS_NOT_NULL";

    // <expression> IS DISTINCT FROM <expression>
    // ▶ IS_DISTINCT_FROM(<expression>, <expression>)
    public const string IsDistinctFrom = "IS_DISTINCT_FROM";

    // <expression> IS NOT DISTINCT FROM <expression>
    // ▶ IS_NOT_DISTINCT_FROM(<expression>, <expression>)
    public const string IsNotDistinctFrom = "IS_NOT_DISTINCT_FROM";

    // <expression> AT TIME ZONE {<interval> | <string>}
    // ▶ AT_TIME_ZONE(<expression>, {<interval> | <string>})
    public const string AtTimeZone = "AT_TIME_ZONE";

    // POSITION (<expression> IN <expression>)
    // ▶ POSITION(<expression>, <expression>)
    public const string Position = "POSITION";

    // FILTER ( WHERE <expression> )
    // ▶ FILTER(<expression>)
    public const string Filter = "FILTER";
}
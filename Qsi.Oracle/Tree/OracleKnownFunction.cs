namespace Qsi.Oracle.Tree;

public static class OracleKnownFunction
{
    // <expression> IS NULL
    // ▶ IS_NULL(<expression>)
    public const string IsNull = "IS_NULL";

    // <expression> IS NOT NULL
    // ▶ IS_NOT_NULL(<expression>)
    public const string IsNotNull = "IS_NOT_NULL";

    // <expression> IS NOT INFINITE
    // ▶ IS_NOT_INFINITE(<expression>)
    public const string IsNotInfinite = "IS_NOT_INFINITE";

    // <expression> IS INFINITE
    // ▶ IS_INFINITE(<expression>)
    public const string IsInfinite = "IS_INFINITE";

    // <expression> IS NOT NAN
    // ▶ IS_NOT_NAN(<expression>)
    public const string IsNotNaN = "IS_NOT_NAN";

    // <expression> IS NAN
    // ▶ IS_NAN(<expression>)
    public const string IsNaN = "IS_NAN";

    // <expression> IS NOT A SET
    // ▶ IS_NOT_NAN(<expression>)
    public const string IsNotASet = "IS_NOT_A_SET";

    // <expression> IS A SET
    // ▶ IS_NAN(<expression>)
    public const string IsASet = "IS_A_SET";

    // <expression> IS NOT EMPTY
    // ▶ IS_NOT_NAN(<expression>)
    public const string IsNotEmpty = "IS_NOT_EMPTY";

    // <expression> IS EMPTY
    // ▶ IS_NAN(<expression>)
    public const string IsEmpty = "IS_EMPTY";

    // <expression> IS NOT DANGLING
    // ▶ IS_NOT_DANGLING(<expression>)
    public const string IsNotDangling = "IS_NOT_DANGLING";

    // <expression> IS NAN
    // ▶ IS_DANGLING(<expression>)
    public const string IsDangling = "IS_DANGLING";

    // [<identifier> IS] ANY
    // ▶ IS_ANY(<identifier>)
    public const string IsAny = "IS_ANY";

    // <expression> LIKE <expression> [ESCAPE <expression>]
    // ▶ LIKE(<expression>, <expression>[, <expression>])
    public const string Like = "LIKE";
    public const string LikeC = "LIKEC";
    public const string Like2 = "LIKE2";
    public const string Like4 = "LIKE4";

    // <expression> NOT LIKE <expression> [ESCAPE <expression>]
    // ▶ NOT_LIKE(<expression>, <expression>[, <expression>])
    public const string NotLike = "NOT_LIKE";
    public const string NotLikeC = "NOT_LIKEC";
    public const string NotLike2 = "NOT_LIKE2";
    public const string NotLike4 = "NOT_LIKE4";

    public const string RegexpLike = "REGEXP_LIKE";

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

    // <expression> IS NOT OF TYPE (<type>)
    // ▶ IS_NOT_OF_TYPE(<expression>, <type>)
    public const string IsNotOfType = "IS_NOT_OF_TYPE";

    // <expression> IS OF TYPE (<type>)
    // ▶ IS_OF_TYPE(<expression>, <type>)
    public const string IsOfType = "IS_OF_TYPE";

    // <expression> IS JSON ...
    // ▶ IS_JSON(<expression>)
    public const string IsJson = "IS_JSON";

    // <expression> IS NOT JSON ...
    // ▶ IS_NOT_JSON(<expression>)
    public const string IsNotJson = "IS_NOT_JSON";

    // GROUPING SETS ( {rollupCubeClause|groupingExpressionList} )
    // ▶ GROUPING_SETS( {rollupCubeClause|groupingExpressionList} )
    public const string GroupingSets = "GROUPING_SETS";

    // JSON [ jsonObjectContent ]
    // ▶ JSON_ARRAY ( jsonObjectContent )
    public const string JsonArray = "JSON_ARRAY";

    // JSON ( jsonObjectContent )
    // ▶ JSON_OBJECT ( jsonObjectContent )
    public const string JsonObject = "JSON_OBJECT";
}
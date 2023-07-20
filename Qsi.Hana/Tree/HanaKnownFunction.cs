namespace Qsi.Hana.Tree;

public static class HanaKnownFunction
{
    // Window
    public const string Binning = "BINNING";
    public const string CubicSplinePpprox = "CUBIC_SPLINE_APPROX";
    public const string CumeDist = "CUME_DIST";
    public const string DenseRank = "DENSE_RANK";
    public const string Lag = "LAG";
    public const string Lead = "LEAD";
    public const string LinearApprox = "LINEAR_APPROX";
    public const string Ntile = "NTILE";
    public const string PercentRank = "PERCENT_RANK";
    public const string PercentileCont = "PERCENTILE_CONT";
    public const string PercentileDisc = "PERCENTILE_DISC";
    public const string RandomPartition = "RANDOM_PARTITION";
    public const string Rank = "RANK";
    public const string RowNumber = "ROW_NUMBER";
    public const string SeriesFilter = "SERIES_FILTER";
    public const string WeightedAvg = "WEIGHTED_AVG";

    // Aggregate
    public const string Count = "COUNT";

    // COUNT(DISTINCT <expressions>)
    // ▶ COUNT_DISTINCT(<expressions>)
    public const string CountDistinct = "COUNT_DISTINCT";

    // Etc

    // CAST(<expression> AS <dataType>)
    // ▶ CAST(<expression>, <dataType>)
    public const string Cast = "CAST";

    // EXTRACT(<YEAR | MONTH | DAY | HOUR | MINUTE | SECOND> FROM <expression>)
    // ▶ EXTRACT(<YEAR | MONTH | DAY | HOUR | MINUTE | SECOND>, <expression>)
    public const string Extract = "EXTRACT";

    // CURRENT OF <identifier>
    // ▶ CURRENT_OF(<identifier>)
    public const string CurrentOf = "CURRENT_OF";

    // <expression> BETWEEN <expression> AND <expression>
    // ▶ BETWEEN(<expression>, <expression>, <expression>)
    public const string Between = "BETWEEN";

    // <expression> NOT BETWEEN <expression> AND <expression>
    // ▶ NOT_BETWEEN(<expression>, <expression>, <expression>)
    public const string NotBetween = "NOT_BETWEEN";

    public const string Contains = "Contains";

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

    // <expression> MEMBER OF <expression>
    // ▶ MEMBER_OF(<expression>, <expression>)
    public const string MemberOf = "MEMBER_OF";

    // <expression> NOT MEMBER OF <expression>
    // ▶ MEMBER_OF(<expression>, <expression>)
    public const string NotMemberOf = "NOT_MEMBER_OF";

    // <expression> IS NULL
    // ▶ IS_NULL(<expression>)
    public const string IsNull = "IS_NULL";

    // <expression> IS NOT NULL
    // ▶ IS_NOT_NULL(<expression>)
    public const string IsNotNull = "IS_NOT_NULL";

    public const string LikeRegexpr = "LIKE_REGEXPR";

    // EXISTS (<subquery>)
    // ▶ EXISTS(<subquery>)
    public const string Exists = "EXISTS";

    // NOT EXISTS (<subquery>)
    // ▶ NOT_EXISTS(<subquery>)
    public const string NotExists = "NOT_EXISTS";

    public const string CrossCorr = "CROSS_CORR";
    public const string StringAgg = "STRING_AGG";
    public const string Dft = "DFT";
}
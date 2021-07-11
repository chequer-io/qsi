namespace Qsi.Impala.Tree
{
    internal static class ImpalaKnownFunction
    {
        // <expression> IS NULL
        // ▶ IS_NULL(<expression>)
        public const string IsNull = "IS_NULL";

        // <expression> IS NOT NULL
        // ▶ IS_NOT_NULL(<expression>)
        public const string IsNotNull = "IS_NOT_NULL";

        // CAST(<expr> AS <type_def> [cast_format_val])
        // ▶ CAST(<expr>, <type_def>)
        public const string Cast = "CAST";

        // <func> OVER ([partition_clause] [order_by_clause] [opt_window_clause])
        // ▶ ANALYTIC(<func>)
        public const string Analytic = "ANALYTIC";

        // INTERVAL <expression> <identifier> + <expression>
        public const string Interval = "INTERVAL";
    }
}

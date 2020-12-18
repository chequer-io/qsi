namespace Qsi.MySql.Tree
{
    public static class MySqlKnownFunction
    {
        // <identifier> IS NULL
        // ▶ IS_NULL(<identifier>)
        public const string IsNull = "IS_NULL";

        // <identifier> IS NOT NULL
        // ▶ IS_NOT_NULL(<identifier>)
        public const string IsNotNull = "IS_NOT_NULL";

        // INTERVAL <expression> <interval>
        // ▶ INTERVAL(<expression>, <interval>)
        public const string Interval = "INTERVAL";

    }
}

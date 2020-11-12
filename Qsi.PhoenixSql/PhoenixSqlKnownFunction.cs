namespace Qsi.PhoenixSql
{
    public static class PhoenixSqlKnownFunction
    {
        // <l_expr> IN (<r_expr>)
        // ▶ IN(<l_expr>, <r_expr>)
        public const string In = "IN";

        // <expr> IN (<element>, ..)
        // ▶ ARRAY_IN(<expr>, <element>, ..)
        public const string ArrayIn = "IN";

        // <l_expr> LIKE <r_expr>
        // ▶ LIKE(<l_expr>, <r_expr>)
        public const string Like = "LIKE";

        // <l_expr> ILIKE <r_expr>
        // ▶ ILIKE(<l_expr>, <r_expr>)
        public const string ILike = "ILIKE";

        // EXISTS <expr>
        // ▶ EXISTS(<expr>)
        public const string Exists = "EXISTS";

        // <expr> IS NULL
        // ▶ IS_NULL(<expr>)
        public const string IsNull = "IS_NULL";

        // CAST(<expr> AS <type>)
        // ▶ CAST(<expr>, <type>)
        public const string Cast = "CAST";

        // ARRAY[<element>, ..]
        // ▶ ARRAY(<element>, ..)
        public const string Array = "ARRAY";

        // <expr> BETWEEN <start> AND <end>
        // ▶ BETWEEN(<expr>, <start>, <end>)
        public const string Between = "BETWEEN";

        // CURRENT VALUE FOR <expr>
        // ▶ CURRENT_VALUE_FOR(<expr>)
        public const string CurrentValueFor = "CURRENT_VALUE_FOR";

        // NEXT VALUE FOR <expr>
        // ▶ NEXT_VALUE_FOR(<expr>)
        public const string NextValueFor = "NEXT_VALUE_FOR";
    }
}

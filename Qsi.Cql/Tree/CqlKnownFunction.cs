namespace Qsi.Cql.Tree
{
    public sealed class CqlKnownFunction
    {
        public const string Count = "COUNT";

        public const string Writetime = "WRITETIME";

        public const string TTL = "TTL";

        // CAST(<expr> AS <type>)
        // ▶ CAST(<expr>, <type>)
        public const string Cast = "CAST";

        // (<type>) <expr>
        // ▶ CAST_INLINE(<expr>, <type>)
        public const string InlineCast = "CAST_INLINE";

        public const string Expr = "EXPR";

        // <cident> LIKE <expr>
        // ▶ LIKE(<cident>, <expr>)
        public const string Like = "LIKE";

        // <cident> IS NOT NULL
        // ▶ IS_NOT_NULL(<cident>)
        public const string IsNotNull = "IS_NOT_NULL";

        // TOKEN <l_expr> <op> <r_expr>
        // ▶ TOKEN_COMPARE(<l_expr>, <op>, <r_expr>)
        public const string TokenCompare = "TOKEN_COMPARE";

        // <l_expr> IN <r_expr>
        // ▶ IN(<l_expr>, <r_expr>)
        public const string In = "IN";

        // <l_expr> CONTAINS <r_expr>
        // ▶ CONTAINS(<l_expr>, <r_expr>)
        public const string Contains = "CONTAINS";

        // <l_expr> CONTAINS KEY <r_expr>
        // ▶ CONTAINS_KEY(<l_expr>, <r_expr>)
        public const string ContainsKey = "CONTAINS_KEY";
    }
}

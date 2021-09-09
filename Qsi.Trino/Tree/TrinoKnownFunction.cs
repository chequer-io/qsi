namespace Qsi.Trino.Tree
{
    public static class TrinoKnownFunction
    {
        // GROUPING SETS ( <groupingSet>, .. )
        // ▶ GROUPING_SETS(<groupingSet>, ..)
        public const string GroupingSets = "GROUPING_SETS";
        
        // TRY_CAST ( expression AS type )
        // ▶ CAST ( expression AS type)
        public const string Cast = "CAST";
    }
}

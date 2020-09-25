namespace Qsi.Analyzers
{
    public sealed class QsiAnalyzerOptions
    {
        /// <summary>
        /// Allow empty columns in <c>select statement</c>.
        /// </summary>
        public bool AllowEmptyColumnsInSelect { get; set; }

        /// <summary>
        /// Allow empty columns in <c>inline statement</c>.
        /// </summary>
        public bool AllowEmptyColumnsInInline { get; set; }

        /// <summary>
        /// Allow no alias in derived table.
        /// </summary>
        public bool AllowNoAliasInDerivedTable { get; set; }

        /// <summary>
        /// Use auto fix invalid recursive query
        /// </summary>
        public bool UseAutoFixRecursiveQuery { get; set; }

        /// <summary>
        /// Use explicit relation access
        /// </summary>
        public bool UseExplicitRelationAccess { get; set; }
    }
}

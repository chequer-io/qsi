namespace Qsi.Compiler
{
    public sealed class QsiTableCompileOptions
    {
        /// <summary>
        /// Allow empty columns in <c>select statement</c>.
        /// </summary>
        public bool AllowEmptyColumnsInSelect { get; set; }

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

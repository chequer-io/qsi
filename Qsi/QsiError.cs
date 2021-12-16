namespace Qsi
{
    public enum QsiError
    {
        Internal,
        NotSupportedFeature,
        NotSupportedScript,
        NotSupportedTree,
        Syntax,
        SyntaxError,
        UnableResolveTable,
        UnableResolveColumn,
        UnknownTable,
        UnknownTableIn,
        UnknownView,
        UnknownViewIn,
        UnknownColumn,
        UnknownColumnIn,
        AmbiguousColumnIn,
        DifferentColumnsCount,
        SpecifiesMoreColumnNames,
        DuplicateColumnName,
        NoTablesUsed,
        NoAnchorInRecursiveQuery,
        NoTopLevelUnionInRecursiveQuery,
        NoFromClause,
        UnableResolveDefinition,
        NoColumnsSpecified,
        NoAlias,
        UnknownVariable,
        NotUpdatableColumn,
        DifferentColumnValueCount,
        ParameterIndexOutOfRange,
        ParameterNotFound,
        InvalidNestedExplain,
        SubqueryReturnsMoreThanRow
    }

    internal static class SR
    {
        public const string Internal = "{0}";
        public const string NotSupportedFeature = "'{0}' is not supported feature";
        public const string NotSupportedScript = "Not supported script type '{0}'";
        public const string NotSupportedTree = "Not supported tree type '{0}'";
        public const string Syntax = "You have an error in your SQL syntax; check the manual that corresponds to your Database server version";
        public const string SyntaxError = "{0}";
        public const string UnableResolveTable = "Unable to resolve table '{0}'";
        public const string UnableResolveColumn = "Unable to resolve column '{0}'";
        public const string UnknownTable = "Unknown table '{0}'";
        public const string UnknownTableIn = "Unknown table '{0}' in {1}";
        public const string UnknownView = "Unknown view '{0}'";
        public const string UnknownViewIn = "Unknown view '{0}' in {1}";
        public const string UnknownColumn = "Unknown column '{0}'";
        public const string UnknownColumnIn = "Unknown column '{0}' in {1}";
        public const string AmbiguousColumnIn = "Column '{0}' in {1} is ambiguous";
        public const string DifferentColumnsCount = "The used Statements have a different number of columns.";
        public const string SpecifiesMoreColumnNames = "Specifies more column names than columns.";
        public const string DuplicateColumnName = "Duplicate column name '{0}'";
        public const string NoTablesUsed = "No tables used";
        public const string NoAnchorInRecursiveQuery = "No anchor member was specified for recursive query '{0}'.";
        public const string NoTopLevelUnionInRecursiveQuery = "Recursive common table expression '{0}' does not contain a top-level UNION ALL operator.";
        public const string NoFromClause = "FROM clause not found where expected.";
        public const string UnableResolveDefinition = "Unable to resolve definition '{0}'";
        public const string NoColumnsSpecified = "No columns specified in '{0}'";
        public const string NoAlias = "Every derived table must have an alias";
        public const string UnknownVariable = "Unknown variable '{0}'";
        public const string NotUpdatableColumn = "Column '{0}' is not updatable";
        public const string DifferentColumnValueCount = "Column count doesn't match value count at row {0}";
        public const string ParameterIndexOutOfRange = "Parameter index {0} was out of range";
        public const string ParameterNotFound = "Parameter '{0}' not found";
        public const string InvalidNestedExplain = "Invalid nested explain for '{0}'";
        public const string SubqueryReturnsMoreThanRow = "Subquery returns more than {0} row";

        public static string GetResource(QsiError error)
        {
            return error switch
            {
                QsiError.Internal => Internal,
                QsiError.NotSupportedFeature => NotSupportedFeature,
                QsiError.NotSupportedScript => NotSupportedScript,
                QsiError.NotSupportedTree => NotSupportedTree,
                QsiError.Syntax => Syntax,
                QsiError.SyntaxError => SyntaxError,
                QsiError.UnableResolveTable => UnableResolveTable,
                QsiError.UnableResolveColumn => UnableResolveColumn,
                QsiError.UnknownTable => UnknownTable,
                QsiError.UnknownTableIn => UnknownTableIn,
                QsiError.UnknownView => UnknownView,
                QsiError.UnknownViewIn => UnknownViewIn,
                QsiError.UnknownColumn => UnknownColumn,
                QsiError.UnknownColumnIn => UnknownColumnIn,
                QsiError.AmbiguousColumnIn => AmbiguousColumnIn,
                QsiError.DifferentColumnsCount => DifferentColumnsCount,
                QsiError.SpecifiesMoreColumnNames => SpecifiesMoreColumnNames,
                QsiError.DuplicateColumnName => DuplicateColumnName,
                QsiError.NoTablesUsed => NoTablesUsed,
                QsiError.NoAnchorInRecursiveQuery => NoAnchorInRecursiveQuery,
                QsiError.NoTopLevelUnionInRecursiveQuery => NoTopLevelUnionInRecursiveQuery,
                QsiError.NoFromClause => NoFromClause,
                QsiError.UnableResolveDefinition => UnableResolveDefinition,
                QsiError.NoColumnsSpecified => NoColumnsSpecified,
                QsiError.NoAlias => NoAlias,
                QsiError.UnknownVariable => UnknownVariable,
                QsiError.NotUpdatableColumn => NotUpdatableColumn,
                QsiError.DifferentColumnValueCount => DifferentColumnValueCount,
                QsiError.ParameterIndexOutOfRange => ParameterIndexOutOfRange,
                QsiError.ParameterNotFound => ParameterNotFound,
                QsiError.InvalidNestedExplain => InvalidNestedExplain,
                QsiError.SubqueryReturnsMoreThanRow => SubqueryReturnsMoreThanRow,
                _ => null
            };
        }
    }
}

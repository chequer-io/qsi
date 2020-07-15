namespace Qsi
{
    public enum QsiError
    {
        Internal,
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
        NoTablesUsed
    }

    internal static class SR
    {
        public const string Internal = "{0}";
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
        public const string NoTablesUsed = "No tables used";

        public static string GetResource(QsiError error)
        {
            return error switch
            {
                QsiError.Internal => Internal,
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
                QsiError.NoTablesUsed => NoTablesUsed,
                _ => null
            };
        }
    }
}

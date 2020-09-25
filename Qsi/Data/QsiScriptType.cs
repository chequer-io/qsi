namespace Qsi.Data
{
    public enum QsiScriptType
    {
        Unknown,

        // ** DML **
        With,
        Select,
        Insert,
        Update,
        Delete,

        // ** DDL **
        Create,
        Alter,
        Drop,
        Rename,
        Truncate,

        // ** DCL **
        Grant,
        Revoke,

        // ** TCL **
        Commit,
        Rollback,
        SavePoint,

        // Others
        Delimiter,
        Call,
        Execute,
        Explain,
        Use,
        Show,
        Describe,

        Comment
    }
}

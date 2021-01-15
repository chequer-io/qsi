namespace Qsi.Data
{
    public enum QsiScriptType
    {
        Unknown,

        // ** DML **
        With,
        Select,
        Insert,
        Upsert,
        Replace,
        Update,
        Delete,
        Merge,

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

        // ** Prepared **
        Prepare,
        DropPrepare,
        Execute,

        // Others
        Delimiter,
        Call,
        Explain,
        Use,
        Show,
        Describe,
        Set,
        Begin,

        Comment
    }
}

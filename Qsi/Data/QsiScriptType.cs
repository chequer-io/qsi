namespace Qsi.Data
{
    public enum QsiScriptType
    {
        Unknown,

        // ** DML **
        DataManipulation,
        Select,
        Insert,
        Update,
        Delete,

        // ** DDL **
        DataDefinition,
        CreateTable,
        CreateView,
        CreateFunction,
        CreateProcedure,
        AlterTable,
        AlterView,
        AlterFunction,
        AlterProcedure,
        DropTable,
        DropView,
        DropFunction,
        DropProcedure

        // ** TCL **
        // TODO: Transaction Control Language

        // ** DCL **
        // TODO: Data Control Language
    }
}

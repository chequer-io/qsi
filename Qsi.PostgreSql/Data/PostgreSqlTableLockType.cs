namespace Qsi.PostgreSql.Data;

public enum PostgreSqlTableLockType
{
    Update,
    NoKeyUpdate,
    Share,
    KeyShare
}

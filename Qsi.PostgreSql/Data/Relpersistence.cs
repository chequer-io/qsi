namespace Qsi.PostgreSql.Data;

public enum Relpersistence
{
    Unknown = 0,

    // regular table
    Permanent = 'p',

    // unlogged permanent table
    Unlogged = 'u',

    // temporary table
    Temp = 't',
}

using System;

namespace Qsi.Oracle
{
    internal static class OraclePseudoColumn
    {
        public static readonly string[] Names =
        {
            "ROWNUM",
            "ROWID",
            "ORA_ROWSCN",
            "COLUMN_VALUE",

            // Hierarchical Query
            "LEVEL",
            "CONNECT_BY_ISLEAF",
            "CONNECT_BY_ISCYCLE",

            // Version Query
            "VERSIONS_STARTTIME",
            "VERSIONS_STARTSCN",
            "VERSIONS_ENDTIME",
            "VERSIONS_ENDSCN",
            "VERSIONS_XID",
            "VERSIONS_OPERATION"
        };

        public static bool Contains(string name)
        {
            return Array.IndexOf(Names, name) >= 0;
        }
    }
}

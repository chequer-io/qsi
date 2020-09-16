using System;

namespace Qsi.Oracle
{
    internal static class OracleFunction
    {
        public static readonly string[] Names =
        {
            "CURRENT_DATE",
            "CURRENT_TIMESTAMP",
            "DBTIMEZONE",
            "ITERATION_NUMBER",
            "LOCALTIMESTAMP",
            "SESSIONTIMEZONE",
            "SYSDATE",
            "SYSTIMESTAMP",
            "UID",
            "USER"
        };

        public static bool Contains(string name)
        {
            return Array.IndexOf(Names, name) >= 0;
        }
    }
}

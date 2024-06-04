using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;

namespace Qsi.SingleStore.Internal;

internal static class SingleStoreUtility
{
    // backend/wbpublic/grtdb/db_helpers.cpp
    // bec::version_to_int
    //
    // Version  |  GrtVersionRef
    // ---------+---------------
    // Major    |  Major
    // Minor    |  Minor
    // Build    |  Release
    // Revision |  Build
    public static int VersionToInt(Version version)
    {
        if (version == null || version.Major == -1)
            return 80000;

        int result = version.Major * 10000;

        if (version.Minor > 0)
            result += version.Minor * 100;

        if (version.Build > 0)
            result += version.Build;

        return result;
    }
}

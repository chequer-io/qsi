using System.Collections.Generic;

namespace Qsi.Utilities;

public static class ListUtility
{
    public static bool IsNullOrEmpty<T>(IList<T> list)
    {
        return list == null || list.Count == 0;
    }
}
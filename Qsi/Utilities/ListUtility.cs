using System.Collections;

namespace Qsi.Utilities
{
    public static class ListUtility
    {
        public static bool IsNullOrEmpty(IList list)
        {
            return list == null || list.Count == 0;
        }
    }
}

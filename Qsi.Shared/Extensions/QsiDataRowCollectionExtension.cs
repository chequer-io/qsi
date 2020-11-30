using Qsi.Data;

namespace Qsi.Shared.Extensions
{
    internal static class QsiDataRowCollectionExtension
    {
        public static QsiDataRowCollection ToNullIfEmpty(this QsiDataRowCollection collection)
        {
            if (collection == null || collection.Count == 0)
                return null;

            return collection;
        }
    }
}

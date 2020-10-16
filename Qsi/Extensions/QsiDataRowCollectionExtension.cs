using System.Collections;
using System.Collections.Generic;
using Qsi.Data;

namespace Qsi.Extensions
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

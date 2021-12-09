using System;
using System.Collections.Generic;

namespace Qsi.Data.Cache
{
    public interface IQsiDataTableCacheProvider : IDisposable
    {
        int Count { get; }

        QsiDataRow Get(int row);

        IEnumerable<QsiDataRow> Get(int row, int count);

        void Add(QsiDataRow row);

        void Flush();

        void Clear();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Qsi.Data.Cache;

namespace Qsi.Data
{
    public sealed class QsiDataRowCollection : ICollection<QsiDataRow>, IDisposable
    {
        public int Count => _cacheProvider.Count;

        public int ColumnCount { get; }

        public QsiDataRow this[int index] => _cacheProvider.Get(index);

        public bool IsReadOnly => false;

        private IQsiDataTableCacheProvider _cacheProvider;

        public QsiDataRowCollection(int columnCount, IQsiDataTableCacheProvider cacheProvider)
        {
            ColumnCount = columnCount;
            _cacheProvider = cacheProvider;
        }

        public void Add(QsiDataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (row.Length != ColumnCount)
                throw new ArgumentException(nameof(row));

            _cacheProvider.Add(row);
        }

        public void AddRange(IEnumerable<QsiDataRow> rows)
        {
            foreach (var row in rows)
            {
                Add(row);
            }
        }
        
        public void Clear()
        {
            _cacheProvider.Clear();
        }

        public bool Contains(QsiDataRow item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(QsiDataRow[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(QsiDataRow item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<QsiDataRow> GetEnumerator()
        {
            return _cacheProvider.Get(0, Count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        void IDisposable.Dispose()
        {
            _cacheProvider?.Dispose();
            _cacheProvider = null;
        }
    }
}

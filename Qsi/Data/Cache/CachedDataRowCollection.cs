using System;
using System.Collections;
using System.Collections.Generic;

namespace Qsi.Data.Cache
{
    public class CachedDataRowCollection : QsiDataRowCollection
    {
        public override int Count => _cacheProvider.Count;

        public override int ColumnCount { get; }

        public override QsiDataRow this[int index] => _cacheProvider.Get(index);

        public override bool IsReadOnly => false;

        private readonly IQsiDataTableCacheProvider _cacheProvider;

        public CachedDataRowCollection(int columnCount, IQsiDataTableCacheProvider cacheProvider)
        {
            ColumnCount = columnCount;
            _cacheProvider = cacheProvider;
        }

        public override void Add(QsiDataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (row.Length != ColumnCount)
                throw new ArgumentException(nameof(row));

            _cacheProvider.Add(row);
        }

        public override void AddRange(IEnumerable<QsiDataRow> rows)
        {
            foreach (var row in rows)
            {
                Add(row);
            }
        }
        
        public override void Clear()
        {
            _cacheProvider.Clear();
        }

        public override bool Contains(QsiDataRow item)
        {
            throw new NotSupportedException();
        }

        public override void CopyTo(QsiDataRow[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public override bool Remove(QsiDataRow item)
        {
            throw new NotSupportedException();
        }

        public override IEnumerator<QsiDataRow> GetEnumerator()
        {
            return _cacheProvider.Get(0, Count).GetEnumerator();
        }
    }
}

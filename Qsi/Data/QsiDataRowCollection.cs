using System;
using System.Collections;
using System.Collections.Generic;

namespace Qsi.Data
{
    public sealed class QsiDataRowCollection : ICollection<QsiDataRow>
    {
        public int Count => _list.Count;

        public int ColumnCount { get; }

        public QsiDataRow this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        bool ICollection<QsiDataRow>.IsReadOnly => false;

        private readonly List<QsiDataRow> _list;

        public QsiDataRowCollection(int columnCount) : this(columnCount, 0)
        {
        }

        public QsiDataRowCollection(int columnCount, int capacity)
        {
            _list = new List<QsiDataRow>(capacity);
            ColumnCount = columnCount;
        }

        public QsiDataRow NewRow()
        {
            var row = new QsiDataRow(ColumnCount);
            _list.Add(row);
            return row;
        }

        public void Add(QsiDataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (row.Length != ColumnCount)
                throw new ArgumentException(nameof(row));

            _list.Add(row);
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
            _list.Clear();
        }

        public bool Contains(QsiDataRow item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(QsiDataRow[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(QsiDataRow item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<QsiDataRow> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}

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

        public QsiDataRowCollection(int columnCount)
        {
            _list = new List<QsiDataRow>();
            ColumnCount = columnCount;
        }

        public QsiDataRow NewRow()
        {
            var row = new QsiDataRow(ColumnCount);
            _list.Add(row);
            return row;
        }

        void ICollection<QsiDataRow>.Add(QsiDataRow item)
        {
            throw new NotSupportedException();
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

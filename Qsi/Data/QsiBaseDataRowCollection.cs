using System;
using System.Collections.Generic;

namespace Qsi.Data
{
    public class QsiBaseDataRowCollection : QsiDataRowCollection
    {
        public override int Count => _list.Count;

        public override int ColumnCount { get; }

        public override QsiDataRow this[int index] => _list[index];

        public override bool IsReadOnly => false;

        private readonly List<QsiDataRow> _list;

        public QsiBaseDataRowCollection(int columnCount) : this(columnCount, 0)
        {
        }

        public QsiBaseDataRowCollection(int columnCount, int capacity)
        {
            _list = new List<QsiDataRow>(capacity);
            ColumnCount = columnCount;
        }

        public override void Add(QsiDataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (row.Length != ColumnCount)
                throw new ArgumentException(nameof(row));

            _list.Add(row);
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
            _list.Clear();
        }

        public override bool Contains(QsiDataRow item)
        {
            return _list.Contains(item);
        }

        public override void CopyTo(QsiDataRow[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public override bool Remove(QsiDataRow item)
        {
            return _list.Remove(item);
        }

        public override IEnumerator<QsiDataRow> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}

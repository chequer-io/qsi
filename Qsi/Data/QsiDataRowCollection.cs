using System.Collections;
using System.Collections.Generic;

namespace Qsi.Data
{
    public abstract class QsiDataRowCollection : ICollection<QsiDataRow>
    {
        public abstract int Count { get; }
        
        public abstract int ColumnCount { get; }

        public abstract QsiDataRow this[int index] { get; }

        public abstract bool IsReadOnly { get; }

        public abstract void Add(QsiDataRow item);

        public abstract void AddRange(IEnumerable<QsiDataRow> rows);

        public abstract void Clear();

        public abstract bool Contains(QsiDataRow item);

        public abstract void CopyTo(QsiDataRow[] array, int arrayIndex);

        public abstract bool Remove(QsiDataRow item);

        public abstract IEnumerator<QsiDataRow> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<QsiDataRow>)this).GetEnumerator();
        }
    }
}

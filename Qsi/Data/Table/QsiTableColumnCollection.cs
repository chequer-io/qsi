using System;
using System.Collections;
using System.Collections.Generic;

namespace Qsi.Data
{
    internal sealed class QsiTableColumnCollection : IList<QsiTableColumn>
    {
        public int Count => _list.Count;

        public QsiTableColumn this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        bool ICollection<QsiTableColumn>.IsReadOnly => false;

        private readonly List<QsiTableColumn> _list;
        private readonly QsiTableStructure _parent;

        public QsiTableColumnCollection(QsiTableStructure parent)
        {
            _list = new List<QsiTableColumn>();
            _parent = parent;
        }

        public void Add(QsiTableColumn item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Parent == _parent)
                return;

            if (item.Parent != null)
                throw new ArgumentException(nameof(item));

            item.Parent = _parent;
            _list.Add(item);
        }

        public void Insert(int index, QsiTableColumn item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Parent == _parent)
                return;

            if (item.Parent != null)
                throw new ArgumentException(nameof(item));

            item.Parent = _parent;
            _list.Insert(index, item);
        }

        public void Clear()
        {
            foreach (var column in _list)
                column.Parent = null;

            _list.Clear();
        }

        public bool Contains(QsiTableColumn item)
        {
            if (item == null || item.Parent != _parent)
                return false;

            return _list.Contains(item);
        }

        public int IndexOf(QsiTableColumn item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Parent != _parent)
                return -1;

            return _list.IndexOf(item);
        }

        public bool Remove(QsiTableColumn item)
        {
            if (item == null)
                return false;

            if (item.Parent != _parent)
                throw new ArgumentException(nameof(item));

            if (!_list.Remove(item))
                return false;

            item.Parent = null;

            return true;
        }

        public void RemoveAt(int index)
        {
            var column = _list[index];

            _list.RemoveAt(index);
            column.Parent = null;
        }

        void ICollection<QsiTableColumn>.CopyTo(QsiTableColumn[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<QsiTableColumn> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

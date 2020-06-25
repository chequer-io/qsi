using System.Collections;
using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public class QsiTreeNodeList<TNode> : IReadOnlyList<TNode> where TNode : QsiTreeNode
    {
        public int Count => _list.Count;

        public TNode this[int index] => _list[index];

        private readonly QsiTreeNode _parent;
        private readonly List<TNode> _list = new List<TNode>(4);

        public QsiTreeNodeList(QsiTreeNode parent)
        {
            _parent = parent;
        }

        public void Add(TNode item)
        {
            _list.Add(item);
            item.Parent = _parent;
        }
        
        public void AddRange(IEnumerable<TNode> items)
        {
            _list.AddRange(items);

            foreach (var item in items)
                item.Parent = _parent;
        }

        public void Clear()
        {
            foreach (var item in _list)
                item.Parent = null;

            _list.Clear();
        }

        public bool Contains(TNode item)
        {
            return _list.Contains(item);
        }

        public bool Remove(TNode item)
        {
            if (_list.Remove(item))
            {
                item.Parent = null;
                return true;
            }

            return false;
        }

        public int IndexOf(TNode item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TNode item)
        {
            _list.Insert(index, item);
            item.Parent = _parent;
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _list.Count)
                _list[index].Parent = null;

            _list.RemoveAt(index);
        }

        public IEnumerator<TNode> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

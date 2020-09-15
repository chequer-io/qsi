using System.Collections;
using System.Collections.Generic;
using java.util;

namespace Qsi.JSql.Extensions
{
    internal static class JavaListExtension
    {
        public static IList<T> AsList<T>(this List list)
        {
            if (list == null)
                return null;

            return new ListWrapper<T>(list);
        }

        private readonly struct ListWrapper<T> : IList<T>
        {
            public int Count => _list.size();

            public bool IsReadOnly { get; }

            public T this[int index]
            {
                get => (T)_list.get(index);
                set => _list.set(index, value);
            }

            private readonly List _list;

            public ListWrapper(List list)
            {
                _list = list;
                IsReadOnly = false;
            }

            public void Add(T item)
            {
                _list.add(item);
            }

            public void Clear()
            {
                _list.clear();
            }

            public bool Contains(T item)
            {
                return _list.contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = (T)_list.get(arrayIndex + i);
                }
            }

            public bool Remove(T item)
            {
                return _list.remove(item);
            }

            public int IndexOf(T item)
            {
                return _list.indexOf(item);
            }

            public void Insert(int index, T item)
            {
                _list.add(index, item);
            }

            public void RemoveAt(int index)
            {
                _list.remove(index);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _list.AsEnumerable<T>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}

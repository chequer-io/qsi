using System.Collections.Generic;

namespace Qsi.Collections
{
    internal readonly ref struct KeyList<TKey, TValue>
    {
        public IList<TValue> this[TKey key]
        {
            get
            {
                if (!_dictionary.TryGetValue(key, out IList<TValue> list))
                {
                    list = new List<TValue>();
                    _dictionary[key] = list;
                }

                return list;
            }
        }

        private readonly Dictionary<TKey, IList<TValue>> _dictionary;

        public KeyList(bool _ = false)
        {
            _dictionary = new Dictionary<TKey, IList<TValue>>();
        }
    }
}

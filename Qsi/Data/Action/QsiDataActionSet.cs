using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qsi.Data
{
    public sealed class QsiActionSet : IQsiAction, IReadOnlyList<IQsiAction>
    {
        public int Count => _items.Length;

        public IQsiAction this[int index] => _items[index];

        private readonly IQsiAction[] _items;

        public QsiActionSet(IQsiAction[] items)
        {
            _items = items;
        }

        public IEnumerator<IQsiAction> GetEnumerator()
        {
            return _items.OfType<IQsiAction>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

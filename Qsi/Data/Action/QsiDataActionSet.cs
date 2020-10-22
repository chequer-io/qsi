using System.Collections.Generic;

namespace Qsi.Data
{
    public sealed class QsiActionSet<T> : List<T>, IQsiActionSet<T> where T : IQsiAction
    {
        public QsiActionSet()
        {
        }

        public QsiActionSet(IEnumerable<T> collection) : base(collection)
        {
        }

        public QsiActionSet(int capacity) : base(capacity)
        {
        }
    }
}

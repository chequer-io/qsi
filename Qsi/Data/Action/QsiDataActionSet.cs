namespace Qsi.Data
{
    public sealed class QsiDataActionSet<T> : IQsiAction where T : IQsiAction
    {
        public T[] Items { get; }

        public QsiDataActionSet(T[] items)
        {
            Items = items;
        }
    }
}

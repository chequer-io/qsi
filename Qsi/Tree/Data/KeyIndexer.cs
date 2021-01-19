namespace Qsi.Tree.Data
{
    public sealed class KeyIndexer<T>
    {
        public T this[IQsiTreeNode node]
        {
            get => node.UserData == null ? default : node.UserData.GetData(_key);
            set => node.UserData?.PutData(_key, value);
        }

        private readonly Key<T> _key;

        public KeyIndexer(Key<T> key)
        {
            _key = key;
        }
    }
}

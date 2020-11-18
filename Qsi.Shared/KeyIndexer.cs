using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.Shared
{
    internal sealed class KeyIndexer<T>
    {
        private readonly Key<T> _key;

        public KeyIndexer(Key<T> key)
        {
            _key = key;
        }

        public T this[IQsiTreeNode node]
        {
            get => node.UserData == null ? default : node.UserData.GetData(_key);
            set => node.UserData?.PutData(_key, value);
        }
    }
}

using PhoenixSql;
using Qsi.Shared;
using Qsi.Tree.Data;

namespace Qsi.PhoenixSql.Internal
{
    internal static class PTree
    {
        public static KeyIndexer<IPhoenixNode> RawNode { get; }

        static PTree()
        {
            RawNode = new KeyIndexer<IPhoenixNode>(new Key<IPhoenixNode>("raw_node"));
        }
    }
}

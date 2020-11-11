using PhoenixSql;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.PhoenixSql.Tree
{
    internal static class PhoenixSqlTree
    {
        public static readonly Key<IPhoenixNode> RawNodeKey = new Key<IPhoenixNode>("raw_node");

        public static void SetRawNode(IQsiTreeNode node, IPhoenixNode rawNode)
        {
            node.UserData?.PutData(RawNodeKey, rawNode);
        }

        public static IPhoenixNode GetRawNode(IQsiTreeNode node)
        {
            return node.UserData?.GetData(RawNodeKey);
        }
    }
}

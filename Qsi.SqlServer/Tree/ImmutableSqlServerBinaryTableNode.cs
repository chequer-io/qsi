using System.Collections.Generic;
using Qsi.SqlServer.Data;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.SqlServer.Tree
{
    public readonly struct ImmutableSqlServerBinaryTableNode : ISqlServerBinaryTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public IQsiTableNode Left { get; }

        public SqlServerBinaryTableType BinaryTableType { get; }

        public IQsiTableNode Right { get; }

        public IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                yield return Left;
                yield return Right;
            }
        }

        public ImmutableSqlServerBinaryTableNode(
            IQsiTreeNode parent,
            IQsiTableNode left,
            SqlServerBinaryTableType binaryTableType,
            IQsiTableNode right,
            IUserDataHolder userData)
        {
            Parent = parent;
            Left = left;
            BinaryTableType = binaryTableType;
            Right = right;
            UserData = userData;
        }
    }
}

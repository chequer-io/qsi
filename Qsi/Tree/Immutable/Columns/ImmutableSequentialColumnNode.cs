using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableSequentialColumnNode : IQsiSequentialColumnNode
    {
        public IQsiTreeNode Parent { get; }

        public int Ordinal { get; }

        public IQsiAliasNode Alias { get; }

        public QsiSequentialColumnType ColumnType { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Alias);

        public ImmutableSequentialColumnNode(
            IQsiTreeNode parent,
            int ordinal,
            IQsiAliasNode alias,
            QsiSequentialColumnType columnType,
            IUserDataHolder userData)
        {
            Parent = parent;
            Ordinal = ordinal;
            Alias = alias;
            ColumnType = columnType;
            UserData = userData;
        }
    }
}

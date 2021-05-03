using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableJoinedTableNode : IQsiJoinedTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableNode Left { get; }

        public string JoinType { get; }

        public bool IsNatural { get; }

        public bool IsComma { get; }

        public IQsiTableNode Right { get; }

        public IQsiColumnsDeclarationNode PivotColumns { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Left, Right, PivotColumns);

        public ImmutableJoinedTableNode(
            IQsiTreeNode parent,
            IQsiTableNode left,
            string joinType,
            bool isNatural,
            bool isComma,
            IQsiTableNode right,
            IQsiColumnsDeclarationNode pivotColumns,
            IUserDataHolder userData)
        {
            Parent = parent;
            Left = left;
            JoinType = joinType;
            IsNatural = isNatural;
            IsComma = isComma;
            Right = right;
            PivotColumns = pivotColumns;
            UserData = userData;
        }
    }
}

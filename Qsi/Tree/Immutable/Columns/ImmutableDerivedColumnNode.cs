using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDerivedColumnNode : IQsiDerivedColumnNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiColumnNode Column { get; }

        public IQsiExpressionNode Expression { get; }

        public IQsiAliasNode Alias { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Column, Expression, Alias);

        public ImmutableDerivedColumnNode(IQsiTreeNode parent, IQsiColumnNode column, IQsiAliasNode alias, IUserDataHolder userData)
        {
            Parent = parent;
            Column = column;
            Expression = null;
            Alias = alias;
            UserData = userData;
        }

        public ImmutableDerivedColumnNode(IQsiTreeNode parent, IQsiExpressionNode expression, IQsiAliasNode alias, IUserDataHolder userData)
        {
            Parent = parent;
            Column = null;
            Expression = expression;
            Alias = alias;
            UserData = userData;
        }
    }
}

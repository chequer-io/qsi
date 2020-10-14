using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutablePrepareActionNode : IQsiPrepareActionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Identifier { get; }

        public IQsiExpressionNode Query { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Query);

        public ImmutablePrepareActionNode(
            IQsiTreeNode parent,
            QsiQualifiedIdentifier identifier,
            IQsiExpressionNode query, 
            IUserDataHolder userData)
        {
            Parent = parent;
            Identifier = identifier;
            Query = query;
            UserData = userData;
        }
    }
}

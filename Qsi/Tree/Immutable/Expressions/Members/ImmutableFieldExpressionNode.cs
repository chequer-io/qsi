using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableFieldExpressionNode : IQsiFieldExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Identifier { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableFieldExpressionNode(IQsiTreeNode parent, QsiQualifiedIdentifier identifier, IUserDataHolder userData)
        {
            Parent = parent;
            Identifier = identifier;
            UserData = userData;
        }
    }
}

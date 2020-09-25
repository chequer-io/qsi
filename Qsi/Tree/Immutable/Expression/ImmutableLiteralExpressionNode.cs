using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableLiteralExpressionNode : IQsiLiteralExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public object Value { get; }

        public QsiLiteralType Type { get; }

        public IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public ImmutableLiteralExpressionNode(IQsiTreeNode parent, object value, QsiLiteralType type)
        {
            Parent = parent;
            Value = value;
            Type = type;
        }
    }
}

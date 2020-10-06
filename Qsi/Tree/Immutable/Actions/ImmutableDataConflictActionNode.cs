using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDataConflictActionNode : IQsiDataConflictActionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Target { get; }

        public IQsiAssignExpressionNode[] Elements { get; }

        public IQsiExpressionNode Condition { get; }

        public IEnumerable<IQsiTreeNode> Children => Elements.Concat(TreeHelper.YieldChildren(Condition));

        public ImmutableDataConflictActionNode(
            IQsiTreeNode parent,
            QsiQualifiedIdentifier target,
            IQsiAssignExpressionNode[] elements,
            IQsiExpressionNode condition)
        {
            Parent = parent;
            Target = target;
            Elements = elements;
            Condition = condition;
        }
    }
}

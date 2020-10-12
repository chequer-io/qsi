using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDataConflictActionNode : IQsiDataConflictActionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Target { get; }

        public IQsiSetColumnExpressionNode[] SetValues { get; }

        public IQsiExpressionNode Condition { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => SetValues.Concat(TreeHelper.YieldChildren(Condition));

        public ImmutableDataConflictActionNode(
            IQsiTreeNode parent,
            QsiQualifiedIdentifier target,
            IQsiSetColumnExpressionNode[] setValues,
            IQsiExpressionNode condition, 
            IUserDataHolder userData)
        {
            Parent = parent;
            Target = target;
            SetValues = setValues;
            Condition = condition;
            UserData = userData;
        }
    }
}

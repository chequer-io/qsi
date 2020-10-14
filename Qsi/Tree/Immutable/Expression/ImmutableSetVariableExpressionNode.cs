using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableSetVariableExpressionNode : IQsiSetVariableExpressionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Target { get; }

        public QsiAssignmentKind AssignmentKind { get; }

        public IQsiExpressionNode Value { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value);

        public ImmutableSetVariableExpressionNode(
            IQsiTreeNode parent,
            QsiQualifiedIdentifier target,
            QsiAssignmentKind assignmentKind,
            IQsiExpressionNode value, 
            IUserDataHolder userData)
        {
            Parent = parent;
            Target = target;
            AssignmentKind = assignmentKind;
            Value = value;
            UserData = userData;
        }
    }
}

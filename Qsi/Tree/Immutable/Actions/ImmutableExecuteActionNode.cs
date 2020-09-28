using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableExecuteActionNode : IQsiExecuteActionNode
    {
        public IQsiTreeNode Parent { get; }

        public QsiQualifiedIdentifier Identifier { get; }

        public IQsiMultipleExpressionNode Variables { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Variables);

        public ImmutableExecuteActionNode(
            IQsiTreeNode parent,
            QsiQualifiedIdentifier identifier,
            IQsiMultipleExpressionNode variables)
        {
            Parent = parent;
            Identifier = identifier;
            Variables = variables;
        }
    }
}

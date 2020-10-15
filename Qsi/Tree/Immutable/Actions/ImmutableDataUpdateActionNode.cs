using System.Linq;
using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Immutable
{
    public readonly struct ImmutableDataUpdateActionNode : IQsiDataUpdateActionNode
    {
        public IQsiTreeNode Parent { get; }

        public IUserDataHolder UserData { get; }

        public IQsiTableNode Target { get; }

        public IQsiSetColumnExpressionNode[] SetValues { get; }

        public IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target).Concat(SetValues);

        public ImmutableDataUpdateActionNode(
            IQsiTreeNode parent,
            IUserDataHolder userData,
            IQsiTableAccessNode target,
            IQsiSetColumnExpressionNode[] setValues)
        {
            Parent = parent;
            UserData = userData;
            Target = target;
            SetValues = setValues;
        }
    }
}

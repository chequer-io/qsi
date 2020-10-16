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
            IQsiTableNode target,
            IQsiSetColumnExpressionNode[] setValues,
            IUserDataHolder userData)
        {
            Parent = parent;
            Target = target;
            SetValues = setValues;
            UserData = userData;
        }
    }
}

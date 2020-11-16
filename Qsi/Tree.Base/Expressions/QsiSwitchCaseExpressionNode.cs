using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiSwitchCaseExpressionNode : QsiExpressionNode, IQsiSwitchCaseExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Condition { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Consequent { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Condition, Consequent);

        #region Explicit
        IQsiExpressionNode IQsiSwitchCaseExpressionNode.Condition => Condition.Value;

        IQsiExpressionNode IQsiSwitchCaseExpressionNode.Consequent => Consequent.Value;
        #endregion

        public QsiSwitchCaseExpressionNode()
        {
            Condition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Consequent = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

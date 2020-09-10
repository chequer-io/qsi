using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiSwitchCaseExpressionNode : QsiExpressionNode, IQsiSwitchCaseExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Condition { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Consequent { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Condition.IsEmpty)
                    yield return Condition.Value;

                if (!Consequent.IsEmpty)
                    yield return Consequent.Value;
            }
        }

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

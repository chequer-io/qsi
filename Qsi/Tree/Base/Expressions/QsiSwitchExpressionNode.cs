using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiSwitchExpressionNode : QsiExpressionNode, IQsiSwitchExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public QsiTreeNodeList<QsiSwitchCaseExpressionNode> Cases { get; }

        #region Explicit
        IQsiExpressionNode IQsiSwitchExpressionNode.Value => Value.GetValue();

        IQsiSwitchCaseExpressionNode[] IQsiSwitchExpressionNode.Cases => Cases.Cast<IQsiSwitchCaseExpressionNode>().ToArray();
        #endregion

        public QsiSwitchExpressionNode()
        {
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Cases = new QsiTreeNodeList<QsiSwitchCaseExpressionNode>(this);
        }
    }
}

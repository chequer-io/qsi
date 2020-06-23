using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiSwitchExpressionNode : QsiExpressionNode, IQsiSwitchExpressionNode
    {
        public QsiExpressionNode Value { get; set; }

        public List<QsiSwitchCaseExpressionNode> Cases { get; } = new List<QsiSwitchCaseExpressionNode>();

        #region Explicit
        IQsiExpressionNode IQsiSwitchExpressionNode.Value => Value;

        IQsiSwitchCaseExpressionNode[] IQsiSwitchExpressionNode.Cases => Cases.Cast<IQsiSwitchCaseExpressionNode>().ToArray();
        #endregion
    }
}

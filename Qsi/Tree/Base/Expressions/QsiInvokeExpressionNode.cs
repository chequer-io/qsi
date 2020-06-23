using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree.Base
{
    public sealed class QsiInvokeExpressionNode : QsiExpressionNode, IQsiInvokeExpressionNode
    {
        public QsiFunctionAccessExpressionNode Member { get; set; }

        public List<QsiExpressionNode> Parameters { get; } = new List<QsiExpressionNode>();

        #region Explicit
        IQsiFunctionAccessExpressionNode IQsiInvokeExpressionNode.Member => Member;

        IQsiExpressionNode[] IQsiInvokeExpressionNode.Parameters => Parameters.Cast<IQsiExpressionNode>().ToArray();
        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiSwitchExpressionNode : QsiExpressionNode, IQsiSwitchExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public QsiTreeNodeList<QsiSwitchCaseExpressionNode> Cases { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Value.IsEmpty)
                    yield return Value.Value;

                foreach (var @case in Cases)
                {
                    yield return @case;
                }
            }
        }

        #region Explicit
        IQsiExpressionNode IQsiSwitchExpressionNode.Value => Value.Value;

        IQsiSwitchCaseExpressionNode[] IQsiSwitchExpressionNode.Cases => Cases.Cast<IQsiSwitchCaseExpressionNode>().ToArray();
        #endregion

        public QsiSwitchExpressionNode()
        {
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Cases = new QsiTreeNodeList<QsiSwitchCaseExpressionNode>(this);
        }
    }
}

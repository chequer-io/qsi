using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiDataConflictActionNode : QsiActionNode, IQsiDataConflictActionNode
    {
        public QsiQualifiedIdentifier Target { get; set; }

        public QsiTreeNodeList<QsiAssignExpressionNode> SetValues { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Condition { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(SetValues, Condition);

        #region Explicit
        IQsiAssignExpressionNode[] IQsiDataConflictActionNode.SetValues => SetValues.Cast<IQsiAssignExpressionNode>().ToArray();

        IQsiExpressionNode IQsiDataConflictActionNode.Condition => Condition.Value;
        #endregion

        public QsiDataConflictActionNode()
        {
            SetValues = new QsiTreeNodeList<QsiAssignExpressionNode>(this);
            Condition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

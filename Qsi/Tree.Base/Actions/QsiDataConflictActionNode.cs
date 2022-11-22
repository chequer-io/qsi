using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiDataConflictActionNode : QsiActionNode, IQsiDataConflictActionNode
    {
        public QsiQualifiedIdentifier Target { get; set; }

        public QsiTreeNodeList<QsiSetColumnExpressionNode> SetValues { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Condition { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(SetValues, Condition);

        #region Explicit
        IQsiSetColumnExpressionNode[] IQsiDataConflictActionNode.SetValues => SetValues.Cast<IQsiSetColumnExpressionNode>().ToArray();

        IQsiExpressionNode IQsiDataConflictActionNode.Condition => Condition.Value;
        #endregion

        public QsiDataConflictActionNode()
        {
            SetValues = new QsiTreeNodeList<QsiSetColumnExpressionNode>(this);
            Condition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

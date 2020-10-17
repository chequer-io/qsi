using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public sealed class QsiCompositeTableNode : QsiTableNode, IQsiCompositeTableNode
    {
        public QsiTreeNodeList<QsiTableNode> Sources { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> OrderExpression { get; }

        public QsiTreeNodeProperty<QsiLimitExpressionNode> LimitExpression { get; }

        public override IEnumerable<IQsiTreeNode> Children => Sources;

        #region Explicit
        IQsiTableNode[] IQsiCompositeTableNode.Sources => Sources.Cast<IQsiTableNode>().ToArray();

        IQsiMultipleOrderExpressionNode IQsiCompositeTableNode.OrderExpression => OrderExpression.Value;

        IQsiLimitExpressionNode IQsiCompositeTableNode.LimitExpression => LimitExpression.Value;
        #endregion

        public QsiCompositeTableNode()
        {
            Sources = new QsiTreeNodeList<QsiTableNode>(this);
            OrderExpression = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            LimitExpression = new QsiTreeNodeProperty<QsiLimitExpressionNode>(this);
        }
    }
}

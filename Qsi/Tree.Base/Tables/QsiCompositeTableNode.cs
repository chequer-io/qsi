using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiCompositeTableNode : QsiTableNode, IQsiCompositeTableNode
    {
        public QsiTreeNodeList<QsiTableNode> Sources { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> Order { get; }

        public QsiTreeNodeProperty<QsiLimitExpressionNode> Limit { get; }

        public override IEnumerable<IQsiTreeNode> Children => Sources;

        #region Explicit
        IQsiTableNode[] IQsiCompositeTableNode.Sources => Sources.Cast<IQsiTableNode>().ToArray();

        IQsiMultipleOrderExpressionNode IQsiCompositeTableNode.Order => Order.Value;

        IQsiLimitExpressionNode IQsiCompositeTableNode.Limit => Limit.Value;
        #endregion

        public QsiCompositeTableNode()
        {
            Sources = new QsiTreeNodeList<QsiTableNode>(this);
            Order = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Limit = new QsiTreeNodeProperty<QsiLimitExpressionNode>(this);
        }
    }
}

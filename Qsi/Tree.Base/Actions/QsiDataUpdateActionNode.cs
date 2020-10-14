using System.Linq;
using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiDataUpdateActionNode : QsiActionNode, IQsiDataUpdateActionNode
    {
        public QsiTreeNodeProperty<QsiTableAccessNode> Target { get; }

        public QsiTreeNodeList<QsiSetColumnExpressionNode> SetValues { get; }

        public QsiTreeNodeProperty<QsiWhereExpressionNode> WhereExpression { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> OrderExpression { get; }

        public QsiTreeNodeProperty<QsiLimitExpressionNode> LimitExpression { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target).Concat(SetValues).Concat(TreeHelper.YieldChildren(WhereExpression, OrderExpression, LimitExpression));

        #region Explicit
        IQsiTableAccessNode IQsiDataUpdateActionNode.Target => Target.Value;

        IQsiSetColumnExpressionNode[] IQsiDataUpdateActionNode.SetValues => SetValues.Cast<IQsiSetColumnExpressionNode>().ToArray();

        IQsiWhereExpressionNode IQsiDataUpdateActionNode.WhereExpression => WhereExpression.Value;

        IQsiMultipleOrderExpressionNode IQsiDataUpdateActionNode.OrderExpression => OrderExpression.Value;

        IQsiLimitExpressionNode IQsiDataUpdateActionNode.LimitExpression => LimitExpression.Value;
        #endregion

        public QsiDataUpdateActionNode()
        {
            Target = new QsiTreeNodeProperty<QsiTableAccessNode>(this);
            SetValues = new QsiTreeNodeList<QsiSetColumnExpressionNode>(this);
            WhereExpression = new QsiTreeNodeProperty<QsiWhereExpressionNode>(this);
            OrderExpression = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            LimitExpression = new QsiTreeNodeProperty<QsiLimitExpressionNode>(this);
        }
    }
}

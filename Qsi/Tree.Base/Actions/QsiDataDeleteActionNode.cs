using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiDataDeleteActionNode : QsiActionNode, IQsiDataDeleteActionNode
    {
        public QsiTreeNodeProperty<QsiTableAccessNode> Target { get; }

        public QsiTreeNodeProperty<QsiWhereExpressionNode> WhereExpression { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Target, WhereExpression);

        #region Explicit
        IQsiTableAccessNode IQsiDataDeleteActionNode.Target => Target.Value;

        IQsiWhereExpressionNode IQsiDataDeleteActionNode.WhereExpression => WhereExpression.Value;
        #endregion

        public QsiDataDeleteActionNode()
        {
            Target = new QsiTreeNodeProperty<QsiTableAccessNode>(this);
            WhereExpression = new QsiTreeNodeProperty<QsiWhereExpressionNode>(this);
        }
    }
}

using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree.Base
{
    public sealed class QsiTableExpressionNode : QsiExpressionNode, IQsiTableExpressionNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Table { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Table);

        #region Explicit
        IQsiTableNode IQsiTableExpressionNode.Table => Table.Value;
        #endregion

        public QsiTableExpressionNode()
        {
            Table = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}

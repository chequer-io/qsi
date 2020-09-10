using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiTableExpressionNode : QsiExpressionNode, IQsiTableExpressionNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Table { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Table.IsEmpty)
                    yield return Table.Value;
            }
        }

        #region Explicit
        IQsiTableNode IQsiTableExpressionNode.Table => Table.Value;
        #endregion

        public QsiTableExpressionNode()
        {
            Table = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}

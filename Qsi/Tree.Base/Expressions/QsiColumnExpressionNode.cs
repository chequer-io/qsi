using System.Collections.Generic;

namespace Qsi.Tree
{
    public sealed class QsiColumnExpressionNode : QsiExpressionNode, IQsiColumnExpressionNode
    {
        public QsiTreeNodeProperty<QsiColumnNode> Column { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Column.IsEmpty)
                    yield return Column.Value;
            }
        }

        #region Explicit
        IQsiColumnNode IQsiColumnExpressionNode.Column => Column.Value;
        #endregion

        public QsiColumnExpressionNode()
        {
            Column = new QsiTreeNodeProperty<QsiColumnNode>(this);
        }
    }
}

using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiJoinedTableNode : QsiTableNode, IQsiJoinedTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Left { get; }

        public QsiJoinType JoinType { get; set; }

        public QsiTreeNodeProperty<QsiTableNode> Right { get; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> PivotColumns { get; }

        #region Explicit
        IQsiTableNode IQsiJoinedTableNode.Left => Left.GetValue();

        QsiJoinType IQsiJoinedTableNode.JoinType => JoinType;

        IQsiTableNode IQsiJoinedTableNode.Right => Right.GetValue();

        IQsiColumnsDeclarationNode IQsiJoinedTableNode.PivotColumns => PivotColumns.GetValue();
        #endregion

        public QsiJoinedTableNode()
        {
            Left = new QsiTreeNodeProperty<QsiTableNode>(this);
            Right = new QsiTreeNodeProperty<QsiTableNode>(this);
            PivotColumns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
        }
    }
}

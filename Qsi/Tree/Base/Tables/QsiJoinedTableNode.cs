using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Base
{
    public sealed class QsiJoinedTableNode : QsiTableNode, IQsiJoinedTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Left { get; }

        public QsiJoinType JoinType { get; set; }

        public QsiTreeNodeProperty<QsiTableNode> Right { get; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> PivotColumns { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Left, Right, PivotColumns);

        #region Explicit
        IQsiTableNode IQsiJoinedTableNode.Left => Left.Value;

        QsiJoinType IQsiJoinedTableNode.JoinType => JoinType;

        IQsiTableNode IQsiJoinedTableNode.Right => Right.Value;

        IQsiColumnsDeclarationNode IQsiJoinedTableNode.PivotColumns => PivotColumns.Value;
        #endregion

        public QsiJoinedTableNode()
        {
            Left = new QsiTreeNodeProperty<QsiTableNode>(this);
            Right = new QsiTreeNodeProperty<QsiTableNode>(this);
            PivotColumns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
        }
    }
}

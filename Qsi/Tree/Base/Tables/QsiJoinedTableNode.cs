using System.Collections.Generic;
using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiJoinedTableNode : QsiTableNode, IQsiJoinedTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Left { get; }

        public QsiJoinType JoinType { get; set; }

        public QsiTreeNodeProperty<QsiTableNode> Right { get; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> PivotColumns { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Left.IsEmpty)
                    yield return Left.Value;

                if (!Right.IsEmpty)
                    yield return Right.Value;

                if (!PivotColumns.IsEmpty)
                    yield return PivotColumns.Value;
            }
        }

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

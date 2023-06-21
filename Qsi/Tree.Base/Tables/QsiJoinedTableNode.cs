using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiJoinedTableNode : QsiTableNode, IQsiJoinedTableNode
{
    public QsiTreeNodeProperty<QsiTableNode> Left { get; }

    public string JoinType { get; set; }

    public bool IsNatural { get; set; }

    public bool IsComma { get; set; }

    public QsiTreeNodeProperty<QsiTableNode> Right { get; }

    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> PivotColumns { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> PivotExpression { get; }

    public override IEnumerable<IQsiTreeNode> Children
        => TreeHelper.YieldChildren(Left, Right, PivotColumns, PivotExpression);

    #region Explicit
    IQsiTableNode IQsiJoinedTableNode.Left => Left.Value;

    IQsiTableNode IQsiJoinedTableNode.Right => Right.Value;

    IQsiColumnsDeclarationNode IQsiJoinedTableNode.PivotColumns => PivotColumns.Value;

    IQsiExpressionNode IQsiJoinedTableNode.PivotExpression => PivotExpression.Value;
    #endregion

    public QsiJoinedTableNode()
    {
        Left = new QsiTreeNodeProperty<QsiTableNode>(this);
        Right = new QsiTreeNodeProperty<QsiTableNode>(this);
        PivotColumns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
        PivotExpression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
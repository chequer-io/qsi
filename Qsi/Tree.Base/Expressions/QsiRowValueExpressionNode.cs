using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiRowValueExpressionNode : QsiExpressionNode, IQsiRowValueExpressionNode
    {
        public QsiTreeNodeList<QsiExpressionNode> ColumnValues { get; }

        public override IEnumerable<IQsiTreeNode> Children => ColumnValues;

        #region Explicit
        IQsiExpressionNode[] IQsiRowValueExpressionNode.ColumnValues => ColumnValues.Cast<IQsiExpressionNode>().ToArray();
        #endregion

        public QsiRowValueExpressionNode()
        {
            ColumnValues = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}

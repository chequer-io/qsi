using System.Collections.Generic;
using Qsi.SqlServer.Data;
using Qsi.Tree;

namespace Qsi.SqlServer.Tree
{
    public class SqlServerBinaryTableNode : QsiTableNode, IQsiCompositeTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Left { get; }

        public SqlServerBinaryTableType BinaryTableType { get; set; }

        public QsiTreeNodeProperty<QsiTableNode> Right { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> OrderExpression { get; }

        public QsiTreeNodeProperty<QsiLimitExpressionNode> LimitExpression { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Left.IsEmpty)
                    yield return Left.Value;

                if (!Right.IsEmpty)
                    yield return Right.Value;
            }
        }

        #region Explicit
        IQsiTableNode[] IQsiCompositeTableNode.Sources
        {
            get
            {
                return new IQsiTableNode[]
                {
                    Left.Value,
                    Right.Value
                };
            }
        }

        IQsiMultipleOrderExpressionNode IQsiCompositeTableNode.Order => OrderExpression.Value;

        IQsiLimitExpressionNode IQsiCompositeTableNode.Limit => LimitExpression.Value;
        #endregion

        public SqlServerBinaryTableNode()
        {
            Left = new QsiTreeNodeProperty<QsiTableNode>(this);
            Right = new QsiTreeNodeProperty<QsiTableNode>(this);
            OrderExpression = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            LimitExpression = new QsiTreeNodeProperty<QsiLimitExpressionNode>(this);
        }
    }
}

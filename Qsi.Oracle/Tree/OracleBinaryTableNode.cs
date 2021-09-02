using System.Collections.Generic;
using Qsi.Oracle.Common;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleBinaryTableNode : QsiTableNode, IQsiCompositeTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Left { get; }

        public OracleBinaryTableType BinaryTableType { get; set; }

        public QsiTreeNodeProperty<QsiTableNode> Right { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> Order { get; }

        public QsiTreeNodeProperty<QsiLimitExpressionNode> Limit { get; }

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

        IQsiMultipleOrderExpressionNode IQsiCompositeTableNode.Order => Order.Value;

        IQsiLimitExpressionNode IQsiCompositeTableNode.Limit => Limit.Value;
        #endregion

        public OracleBinaryTableNode()
        {
            Left = new QsiTreeNodeProperty<QsiTableNode>(this);
            Right = new QsiTreeNodeProperty<QsiTableNode>(this);
            Order = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Limit = new QsiTreeNodeProperty<QsiLimitExpressionNode>(this);
        }
    }
}

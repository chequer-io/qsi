using System.Collections.Generic;
using Qsi.SqlServer.Data;
using Qsi.Tree;

namespace Qsi.SqlServer.Tree
{
    public class SqlServerBinaryTableNode : QsiTableNode, ISqlServerBinaryTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Left { get; }

        public SqlServerBinaryTableType BinaryTableType { get; set; }

        public QsiTreeNodeProperty<QsiTableNode> Right { get; }

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
        IQsiTableNode ISqlServerBinaryTableNode.Left => Left.Value;

        SqlServerBinaryTableType ISqlServerBinaryTableNode.BinaryTableType => BinaryTableType;

        IQsiTableNode ISqlServerBinaryTableNode.Right => Right.Value;
        #endregion

        public SqlServerBinaryTableNode()
        {
            Left = new QsiTreeNodeProperty<QsiTableNode>(this);
            Right = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}

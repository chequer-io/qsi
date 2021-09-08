using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleJsonOperationExpressionNode : QsiExpressionNode
    {
        public string Type { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Left { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Right { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleJsonOperationExpressionNode()
        {
            Left = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Right = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }

    public class OracleJsonKeepOperationExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeList<QsiExpressionNode> Items { get; }

        public override IEnumerable<IQsiTreeNode> Children => Items;

        public OracleJsonKeepOperationExpressionNode()
        {
            Items = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}

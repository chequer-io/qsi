using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleJsonOperationExpressionNode : QsiExpressionNode
    {
        public string Type { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Left { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Right { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Left, Right);

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

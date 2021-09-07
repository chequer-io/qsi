using System.Collections.Generic;
using System.Linq;
using Qsi.Oracle.Common;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleJsonFunctionExpressionNode : OracleInvokeExpressionNode
    {
        public OracleNullBehavior NullBehavior { get; set; }

        public string ReturnType { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Parameters;
    }

    public class OracleJsonElementNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public bool IsFormatted { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleJsonElementNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

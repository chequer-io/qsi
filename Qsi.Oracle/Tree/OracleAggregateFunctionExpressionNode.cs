using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleAggregateFunctionExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiInvokeExpressionNode> Function { get; }

        public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }

        public QsiTreeNodeProperty<OracleMultipleOrderExpressionNode> Order { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Function);

        public OracleAggregateFunctionExpressionNode()
        {
            Function = new QsiTreeNodeProperty<QsiInvokeExpressionNode>(this);
            Partition = new QsiTreeNodeProperty<OraclePartitionExpressionNode>(this);
            Order = new QsiTreeNodeProperty<OracleMultipleOrderExpressionNode>(this);
        }
    }
}

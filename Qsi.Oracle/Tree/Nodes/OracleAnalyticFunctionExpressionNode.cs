using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleAnalyticFunctionExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiInvokeExpressionNode> Function { get; }

        public QsiQualifiedIdentifier WindowName { get; set; }

        public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }

        public QsiTreeNodeProperty<OracleMultipleOrderExpressionNode> Order { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Windowing { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Function);

        public OracleAnalyticFunctionExpressionNode()
        {
            Function = new QsiTreeNodeProperty<QsiInvokeExpressionNode>(this);
            Partition = new QsiTreeNodeProperty<OraclePartitionExpressionNode>(this);
            Order = new QsiTreeNodeProperty<OracleMultipleOrderExpressionNode>(this);
            Windowing = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleAnalyticFunctionExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiInvokeExpressionNode> Function { get; set; }

        public QsiQualifiedIdentifier WindowName { get; set; }

        public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; set; }

        public QsiTreeNodeProperty<OracleMultipleOrderExpressionNode> Order { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Windowing { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

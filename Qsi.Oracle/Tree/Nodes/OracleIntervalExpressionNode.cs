using System.Collections.Generic;
using System.Linq;
using Qsi.Oracle.Common;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleIntervalExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> From { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> To { get; }

        public OracleIntervalCycle Cycle { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> LeadingFieldPrecision { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> FractionalSecondPrecision { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(From, To, LeadingFieldPrecision, FractionalSecondPrecision);

        public OracleIntervalExpressionNode()
        {
            From = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            To = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            LeadingFieldPrecision = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            FractionalSecondPrecision = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Common;

public class AthenaIntervalExpressionNode : QsiExpressionNode
{
    public AthenaIntervalExpressionNode()
    {
        Time = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }

    public QsiTreeNodeProperty<QsiExpressionNode> Time { get; }

    public AthenaIntervalField From { get; set; }

    public AthenaIntervalField To { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Time);
}

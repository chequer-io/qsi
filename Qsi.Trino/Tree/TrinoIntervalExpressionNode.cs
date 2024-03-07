using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Trino.Common;
using Qsi.Utilities;

namespace Qsi.Trino.Tree;

public class TrinoIntervalExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Time { get; }

    public TrinoIntervalField From { get; set; }

    public TrinoIntervalField To { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Time);

    public TrinoIntervalExpressionNode()
    {
        Time = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
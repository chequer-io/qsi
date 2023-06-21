using System.Collections.Generic;
using Qsi.Oracle.Common;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree;

public class OracleLimitExpressionNode : QsiLimitExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> LimitPercent { get; }

    public OracleFetchOption FetchOption { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Limit, Offset, LimitPercent);

    public OracleLimitExpressionNode()
    {
        LimitPercent = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
using Qsi.Tree;

namespace Qsi.Hana.Tree;

public sealed class HanaLimitExpressionNode : QsiLimitExpressionNode
{
    public bool TotalRowCount { get; set; }
}
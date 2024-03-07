using Qsi.Tree;

namespace Qsi.Hana.Tree;

public sealed class HanaArrayComparisonNode : QsiBinaryExpressionNode
{
    public HanaArrayComparisonBehavior Behavior { get; set; }
}

public enum HanaArrayComparisonBehavior
{
    Any,
    Some,
    All
}
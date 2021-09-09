using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public sealed class TrinoOrderExpressionNode : QsiOrderExpressionNode
    {
        public TrinoOrderByNullBehavior? NullBehavior { get; set; }
    }
}

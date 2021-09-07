using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public sealed class TrinoOrderByExpressionNode : QsiOrderExpressionNode
    {
        public TrinoOrderByNullBehavior? NullBehavior { get; set; }
    }
}

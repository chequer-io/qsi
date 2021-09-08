using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public sealed class TrinoGroupingExpressionNode : QsiGroupingExpressionNode
    {
        public TrinoSetQuantifier SetQuantifier { get; set; }
    }
}

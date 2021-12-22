using Qsi.Tree;

namespace Qsi.Athena.Common
{
    public class AthenaOrderExpressionNode : QsiOrderExpressionNode
    {
        public AthenaOrderByNullBehavior? NullBehavior { get; set; }
    }
}

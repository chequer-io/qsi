using Qsi.Impala.Common;
using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaOrderExpressionNode : QsiOrderExpressionNode
    {
        public ImpalaNullsOrder? NullsOrder { get; set; }
    }
}

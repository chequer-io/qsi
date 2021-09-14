using Qsi.Oracle.Common;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleOrderExpressionNode : QsiOrderExpressionNode
    {
        public OracleNullsOrder? NullsOrder { get; set; }
    }
}

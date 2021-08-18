using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleMultipleOrderExpressionNode : QsiMultipleOrderExpressionNode
    {
        public new QsiTreeNodeList<OracleOrderExpressionNode> Orders { get; }
        
        public bool Siblings { get; set; }

        public OracleMultipleOrderExpressionNode()
        {
            Orders = new QsiTreeNodeList<OracleOrderExpressionNode>(this);
        }
    }
}

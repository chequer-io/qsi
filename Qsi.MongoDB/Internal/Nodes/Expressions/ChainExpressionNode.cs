namespace Qsi.MongoDB.Internal.Nodes
{
    public class ChainExpressionNode : BaseNode, IExpressionNode
    {
        public IChainExpressionExpressionNode Expression { get; set; }
    }
    
    // TODO: Implement this interface to SimpleCallExpression, MemberExpression
    public interface IChainExpressionExpressionNode
    {
    }
}
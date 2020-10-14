namespace Qsi.MongoDB.Internal.Nodes
{
    public class ArrayExpressionNode : BaseNode, IExpressionNode
    {
        public IArrayExpressionElementNode[] Elements { get; set; }
    }
    
    // TODO: Impl this interface to BaseExpression, SpreadElement
    public interface IArrayExpressionElementNode
    {
    }
}
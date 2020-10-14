namespace Qsi.MongoDB.Internal.Nodes
{
    public class PropertyNode : BaseNode, INode
    {
        public IExpressionNode Key { get; set; }
        
        public IPropertyValueNode Value { get; set; }
        
        public string Kind { get; set; }
        
        public bool Method { get; set; }
        
        public bool Shorthand { get; set; }
        
        public bool Computed { get; set; }
    }

    // TODO: Impl to BaseExpression, Pattern
    public interface IPropertyValueNode
    {
    }
}
namespace Qsi.MongoDB.Internal.Nodes
{
    public class MemberExpressionNode : BaseNode, IExpressionNode, IPatternNode
    {
        // TODO: MultiType (BaseExpression, Super)
        public object Object { get; set; }
        
        public IExpressionNode Property { get; set; }
        
        public bool Computed { get; set; }
        
        public bool Optional { get; set; }
    }
}
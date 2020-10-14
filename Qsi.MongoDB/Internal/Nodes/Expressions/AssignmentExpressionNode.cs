namespace Qsi.MongoDB.Internal.Nodes
{
    public class AssignmentExpressionNode : BaseNode, IExpressionNode
    {
        public string Operator { get; set; }
        
        public AssignmentExpressionLeftNode Left { get; set; }
        
        public IExpressionNode Right { get; set; }
    }

    // TODO: impl to Pattern, MemberExpression
    public interface AssignmentExpressionLeftNode
    {
    }
}
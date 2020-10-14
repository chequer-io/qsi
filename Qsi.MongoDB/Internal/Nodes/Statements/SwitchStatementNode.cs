namespace Qsi.MongoDB.Internal.Nodes
{
    public class SwitchStatementNode : BaseNode, IStatementNode
    {
        public IExpressionNode Discriminant { get; set; }
        
        public SwitchCaseNode[] Cases { get; set; }
    }
}
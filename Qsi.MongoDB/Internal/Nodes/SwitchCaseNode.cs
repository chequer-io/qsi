namespace Qsi.MongoDB.Internal.Nodes
{
    public class SwitchCaseNode : BaseNode, INode
    {
        public IExpressionNode Test { get; set; }
        
        public IStatementNode[] Consequent { get; set; }
    }
}
namespace Qsi.MongoDB.Internal.Nodes
{
    public class ExpressionStatementNode : BaseNode, IStatementNode
    {
        public IExpressionNode Expression { get; set; }
    }
}
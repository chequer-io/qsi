namespace Qsi.MongoDB.Internal.Nodes
{
    public class ImportExpressionNode : BaseNode, IExpressionNode
    {
        public IExpressionNode Source { get; set; }
    }
}
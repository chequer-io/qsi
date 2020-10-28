namespace Qsi.MongoDB.Internal.Nodes
{
    public interface IClassNode : INode
    {
        IExpressionNode SuperClass { get; set; }
        
        ClassBodyNode Body { get; set; }
    }
}
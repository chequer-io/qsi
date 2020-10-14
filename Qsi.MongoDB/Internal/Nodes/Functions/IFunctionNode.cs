namespace Qsi.MongoDB.Internal.Nodes
{
    public interface IFunctionNode : INode
    {
        IPatternNode[] Params { get; set; }
        
        bool Generator { get; set; }
        
        bool Async { get; set; }
        
        // BlockStatement, BaseExpression
        INode Body { get; set; }
    }
}
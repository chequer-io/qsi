namespace Qsi.MongoDB.Internal.Nodes
{
    public class ObjectExpressionNode : BaseNode, IExpressionNode
    {
        public IObjectExpressionPropertyNode Properties { get; set; }
    }

    // TODO: Impl this interface to Property, SpreadElement
    public interface IObjectExpressionPropertyNode
    {
    }
}
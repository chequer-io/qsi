namespace Qsi.Tree
{
    public interface IQsiDataDeleteActionNode : IQsiActionNode
    {
        IQsiTableNode Target { get; }
    }
}

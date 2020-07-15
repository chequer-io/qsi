namespace Qsi.Tree
{
    public interface IQsiColumnAccessExpressionNode : IQsiMemberAccessExpressionNode
    {
        bool IsAll { get; }
    }
}

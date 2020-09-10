namespace Qsi.Tree
{
    public interface IQsiColumnExpressionNode : IQsiExpressionNode
    {
        IQsiColumnNode Column { get; }
    }
}

namespace Qsi.Tree
{
    public interface IQsiRowValueExpressionNode : IQsiExpressionNode
    {
        IQsiExpressionNode[] ColumnValues { get; }
    }
}

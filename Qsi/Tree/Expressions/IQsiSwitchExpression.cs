namespace Qsi.Tree
{
    public interface IQsiSwitchExpression : IQsiExpression
    {
        IQsiExpression Value { get; }

        IQsiSwitchCaseExpression[] Cases { get; }
    }
}

namespace Qsi.Tree
{
    public interface IQsiSwitchCaseExpression : IQsiExpression
    {
        IQsiExpression Condition { get; }

        IQsiExpression Return { get; }
    }
}

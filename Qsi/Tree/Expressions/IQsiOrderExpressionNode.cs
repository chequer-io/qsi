using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiOrderExpressionNode : IQsiExpressionNode
    {
        QsiSortOrder Order { get; }

        IQsiExpressionNode Expression { get; }
    }
}

using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiBindParameterExpressionNode : IQsiExpressionNode
    {
        QsiParameterType Type { get; }

        string Token { get; }

        string Name { get; }
    }
}

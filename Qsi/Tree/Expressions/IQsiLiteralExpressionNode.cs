using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiLiteralExpressionNode : IQsiExpressionNode
    {
        object Value { get; }

        QsiLiteralType Type { get; }
    }
}

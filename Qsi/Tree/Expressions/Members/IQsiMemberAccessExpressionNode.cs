using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiMemberAccessExpressionNode : IQsiExpressionNode
    {
        QsiQualifiedIdentifier Identifier { get; }
    }
}

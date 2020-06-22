using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiMemberAccessExpression : IQsiExpression
    {
        QsiQualifiedIdentifier Identifier { get; }
    }
}

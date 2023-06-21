namespace Qsi.Tree;

public interface IQsiMemberAccessExpressionNode : IQsiExpressionNode
{
    IQsiExpressionNode Target { get; }

    IQsiExpressionNode Member { get; }
}
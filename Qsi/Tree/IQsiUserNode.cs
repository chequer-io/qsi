using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiUserNode : IQsiTreeNode
{
    QsiQualifiedIdentifier UserName { get; }

    IQsiExpressionNode Password { get; }

    IQsiSetValueExpressionNode[] Properties { get; }
}

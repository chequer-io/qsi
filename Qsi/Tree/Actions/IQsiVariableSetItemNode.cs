using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiVariableSetItemNode : IQsiActionNode
{
    QsiIdentifier Name { get; }

    IQsiExpressionNode Expression { get; }
}

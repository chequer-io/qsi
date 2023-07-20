using Qsi.Data;

namespace Qsi.Tree;

public interface IQsiBindParameterExpressionNode : IQsiExpressionNode
{
    QsiParameterType Type { get; }

    string Prefix { get; }

    bool NoSuffix { get; }

    string Name { get; }

    int? Index { get; }
}
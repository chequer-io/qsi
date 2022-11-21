using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public sealed class PostgreSqlSeriesFunctionTableNode : QsiInlineDerivedTableNode
{
    public QsiTreeNodeProperty<QsiInvokeExpressionNode> Source { get; }

    public PostgreSqlSeriesFunctionTableNode()
    {
        Source = new QsiTreeNodeProperty<QsiInvokeExpressionNode>(this);
    }
}

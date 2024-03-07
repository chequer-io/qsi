using Qsi.Tree;

namespace Qsi.SqlServer.Tree;

public sealed class SqlServerSetColumnExpressionNode : QsiSetColumnExpressionNode
{
    public QsiTreeNodeProperty<QsiVariableExpressionNode> Variable { get; }

    public string Operator { get; set; }

    public SqlServerSetColumnExpressionNode()
    {
        Variable = new QsiTreeNodeProperty<QsiVariableExpressionNode>(this);
    }
}
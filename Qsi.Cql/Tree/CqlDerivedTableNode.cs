using Qsi.Tree;

namespace Qsi.Cql.Tree;

public sealed class CqlDerivedTableNode : QsiDerivedTableNode
{
    public bool IsJson { get; set; }

    public bool IsDistinct { get; set; }

    public bool AllowFiltering { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> PerPartitionLimit { get; }

    public CqlDerivedTableNode()
    {
        PerPartitionLimit = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
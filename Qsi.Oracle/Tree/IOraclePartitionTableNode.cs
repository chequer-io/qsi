using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public interface IOraclePartitionTableNode
    {
        QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }
    }
}

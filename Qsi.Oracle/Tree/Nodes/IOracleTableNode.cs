using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public interface IOracleTableNode
    {
        bool IsOnly { get; set; }

        QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }
    }
}

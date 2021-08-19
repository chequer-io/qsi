using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleTableReferenceNode : QsiTableReferenceNode
    {
        public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }

        public OracleTableReferenceNode()
        {
            Partition = new QsiTreeNodeProperty<OraclePartitionExpressionNode>(this);
        }
    }
}

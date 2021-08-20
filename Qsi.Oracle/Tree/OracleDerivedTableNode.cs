using Qsi.Oracle.Common;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public sealed class OracleDerivedTableNode : QsiDerivedTableNode, IOraclePartitionTableNode
    {
        public string Hint { get; set; }

        public OracleQueryBehavior? QueryBehavior { get; set; }

        public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }

        public OracleDerivedTableNode()
        {
            Partition = new QsiTreeNodeProperty<OraclePartitionExpressionNode>(this);
        }
    }
}

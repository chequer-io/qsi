using Qsi.Oracle.Common;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public sealed class OracleDerivedTableNode : QsiDerivedTableNode, IOracleTableNode
    {
        public bool IsOnly { get; set; }

        public string Hint { get; set; }

        public OracleQueryBehavior? QueryBehavior { get; set; }

        public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> FlashbackQueryClause { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> TableClauses { get; }

        public OracleDerivedTableNode()
        {
            Partition = new QsiTreeNodeProperty<OraclePartitionExpressionNode>(this);
            FlashbackQueryClause = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            TableClauses = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }
}

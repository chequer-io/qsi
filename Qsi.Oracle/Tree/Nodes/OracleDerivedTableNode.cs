using Qsi.Oracle.Common;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public sealed class OracleDerivedTableNode : QsiDerivedTableNode, IOracleTableNode
    {
        public bool IsOnly { get; set; }

        public string Hint { get; set; }

        public OracleQueryBehavior? QueryBehavior { get; set; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> FlashbackQueryClause { get; }

        public QsiTreeNodeProperty<QsiExpressionFragmentNode> TableClauses { get; }

        public QsiTreeNodeProperty<OraclePartitionExpressionNode> Partition { get; }

        public QsiTreeNodeProperty<OracleWindowExpressionNode> Window { get; }

        public OracleDerivedTableNode()
        {
            FlashbackQueryClause = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            TableClauses = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            Partition = new QsiTreeNodeProperty<OraclePartitionExpressionNode>(this);
            Window = new QsiTreeNodeProperty<OracleWindowExpressionNode>(this);
        }
    }
}

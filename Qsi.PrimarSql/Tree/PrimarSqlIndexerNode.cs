using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PrimarSql.Tree
{
    public class PrimarSqlIndexerExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Indexer { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Indexer.IsEmpty)
                    yield return Indexer.Value;
            }
        }

        public PrimarSqlIndexerExpressionNode()
        {
            Indexer = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

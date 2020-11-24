using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlIndexerExpressionNode : QsiExpressionNode
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

        public CqlIndexerExpressionNode()
        {
            Indexer = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}

using Qsi.Tree;

namespace Qsi.PrimarSql.Tree
{
    public sealed class PrimarSqlDerivedTableNode : QsiDerivedTableNode
    {
        public SelectSpec SelectSpec { get; set; }

        public QsiTreeNodeProperty<PrimarSqlStartKeyExpressionNode> StartKey { get; set; }

        public PrimarSqlDerivedTableNode()
        {
            StartKey = new QsiTreeNodeProperty<PrimarSqlStartKeyExpressionNode>(this);
        }
    }
}

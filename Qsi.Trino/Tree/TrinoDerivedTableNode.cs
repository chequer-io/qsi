using Qsi.Tree;

namespace Qsi.Trino.Tree
{
    public sealed class TrinoDerivedTableNode : QsiDerivedTableNode
    {
        public TrinoSetQuantifier SetQuantifier { get; set; }

        public QsiTreeNodeProperty<TrinoWindowExpressionNode> Window { get; }

        public TrinoDerivedTableNode()
        {
            Window = new QsiTreeNodeProperty<TrinoWindowExpressionNode>(this);
        }
    }
}

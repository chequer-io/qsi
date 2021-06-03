using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaDerivedTableNode : QsiDerivedTableNode
    {
        // TOP 123
        public ulong? Top { get; set; }

        // DISTINCT | ALL
        public HanaResultSetOperation? Operation { get; set; }

        // TABLESAMPLE [BERNOULLI | SYSTEM] (123)
        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Sampling { get; }

        // FOR ...
        public QsiTreeNodeProperty<HanaTableBehaviorNode> Behavior { get; }

        // AS OF COMMIT ID 123
        // AS OF UTCTIMESTAMP '123'
        public QsiTreeNodeProperty<QsiExpressionFragmentNode> TimeTravel { get; }

        // WITH HINT (ROUTE_TO(1, 2), ...)
        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Hint { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                foreach (var child in base.Children)
                    yield return child;

                if (!Behavior.IsEmpty)
                    yield return Behavior.Value;
            }
        }

        public HanaDerivedTableNode()
        {
            Sampling = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            Behavior = new QsiTreeNodeProperty<HanaTableBehaviorNode>(this);
            TimeTravel = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            Hint = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }
}

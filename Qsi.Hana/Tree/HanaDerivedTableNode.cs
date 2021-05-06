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
        public string Sampling { get; set; }

        public QsiTreeNodeProperty<HanaTableBehaviorNode> Behavior { get; }

        // AS OF COMMIT ID 123
        // AS OF UTCTIMESTAMP '123'
        public string TimeTravel { get; set; }

        // WITH HINT (ROUTE_TO(1, 2), ...)
        public string Hint { get; set; }

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
            Behavior = new QsiTreeNodeProperty<HanaTableBehaviorNode>(this);
        }
    }
}

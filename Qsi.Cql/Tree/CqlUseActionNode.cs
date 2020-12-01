using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlUseActionNode : QsiActionNode
    {
        public QsiIdentifier Identifier { get; set; }

        public override IEnumerable<IQsiTreeNode> Children { get; } = Enumerable.Empty<IQsiTreeNode>();
    }
}

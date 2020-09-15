using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiTableAccessNode : QsiTableNode, IQsiTableAccessNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

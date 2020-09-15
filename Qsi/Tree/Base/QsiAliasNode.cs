using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiAliasNode : QsiTreeNode, IQsiAliasNode
    {
        public QsiIdentifier Name { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

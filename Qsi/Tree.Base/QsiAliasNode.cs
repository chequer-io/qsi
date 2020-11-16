using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree
{
    public class QsiAliasNode : QsiTreeNode, IQsiAliasNode, IQsiTerminalNode
    {
        public QsiIdentifier Name { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

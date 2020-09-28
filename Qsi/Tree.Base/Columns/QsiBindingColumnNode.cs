using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public sealed class QsiBindingColumnNode : QsiColumnNode, IQsiBindingColumnNode, IQsiTerminalNode
    {
        public string Id { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

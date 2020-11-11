using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree
{
    public sealed class QsiDynamicColumnNode : QsiColumnNode, IQsiDynamicColumnNode, IQsiTerminalNode
    {
        public QsiQualifiedIdentifier Name { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

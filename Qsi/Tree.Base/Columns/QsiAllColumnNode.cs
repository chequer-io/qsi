using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree
{
    public class QsiAllColumnNode : QsiColumnNode, IQsiAllColumnNode, IQsiTerminalNode
    {
        public QsiQualifiedIdentifier Path { get; set; }

        public bool IncludeInvisibleColumns { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

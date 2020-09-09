using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiAllColumnNode : QsiColumnNode, IQsiAllColumnNode
    {
        public QsiQualifiedIdentifier Path { get; set; }

        public bool IncludeInvisibleColumns { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

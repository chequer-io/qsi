using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiLiteralExpressionNode : QsiExpressionNode, IQsiLiteralExpressionNode
    {
        public object Value { get; set; }

        public QsiLiteralType Type { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

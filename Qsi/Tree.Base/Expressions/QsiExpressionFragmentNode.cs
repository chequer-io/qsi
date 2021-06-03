using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiExpressionFragmentNode : QsiExpressionNode, IQsiTerminalNode
    {
        public string Value { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public override string ToString()
        {
            return Value;
        }
    }
}

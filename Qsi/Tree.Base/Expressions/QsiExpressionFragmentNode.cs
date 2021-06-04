using System.Collections.Generic;
using System.Linq;

namespace Qsi.Tree
{
    public class QsiExpressionFragmentNode : QsiExpressionNode, IQsiTerminalNode
    {
        public string Text { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public override string ToString()
        {
            return Text;
        }
    }
}

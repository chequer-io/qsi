using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree
{
    public class QsiBindParameterExpressionNode : QsiExpressionNode, IQsiBindParameterExpressionNode
    {
        public QsiParameterType Type { get; set; }

        public string Token { get; set; }

        public string Name { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

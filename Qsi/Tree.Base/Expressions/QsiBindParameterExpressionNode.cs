using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree
{
    public sealed class QsiBindParameterExpressionNode : QsiExpressionNode, IQsiBindParameterExpressionNode
    {
        public QsiParameterType Type { get; set; }

        public string Prefix { get; set; }

        public bool NoSuffix { get; set; }

        public string Name { get; set; }

        public int? Index { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}

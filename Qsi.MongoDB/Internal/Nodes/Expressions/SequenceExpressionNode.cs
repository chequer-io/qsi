using System.Collections.Generic;
using System.Linq;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class SequenceExpressionNode : BaseNode, IExpressionNode
    {
        public IExpressionNode[] Expressions { get; set; }

        public override IEnumerable<INode> Children => Expressions;
    }
}
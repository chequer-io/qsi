using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class SequenceExpressionNode : BaseNode, IExpressionNode
    {
        public IExpressionNode[] Expressions { get; set; }

        public override IEnumerable<INode> Children => Expressions;
    }
}
using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class CallExpressionNode : BaseNode, IExpressionNode
    {
        // BaseExpression, Super
        public INode Callee { get; set; }
        
        // BaseExpression, SpreadElement
        public INode[] Arguments { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Callee;

                foreach (var argument in Arguments)
                    yield return argument;
            }
        }
    }
}
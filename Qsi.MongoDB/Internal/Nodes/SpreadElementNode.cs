using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class SpreadElementNode : BaseNode, INode
    {
        public IExpressionNode Argument { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Argument;
            }
        }
    }
}
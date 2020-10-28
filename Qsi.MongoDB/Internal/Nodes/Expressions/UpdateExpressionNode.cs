using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class UpdateExpressionNode : BaseNode, IExpressionNode
    {
        public string Operator { get; set; }
        
        public IExpressionNode Argument { get; set; }
        
        public bool Prefix { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Argument;
            }
        }
    }
}
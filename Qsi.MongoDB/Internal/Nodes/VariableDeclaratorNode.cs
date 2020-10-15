using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class VariableDeclaratorNode : BaseNode, INode
    {
        public IPatternNode Id { get; set; }
        
        public IExpressionNode Init { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Id;
                yield return Init;
            }
        }
    }
}
using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class ClassExpressionNode : BaseNode, IClassNode, IExpressionNode
    {
        public IExpressionNode SuperClass { get; set; }
        
        public ClassBodyNode Body { get; set; }
        
        public IdentifierNode Id { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return SuperClass;
                yield return Body;
                yield return Id;
            }
        }
    }
}
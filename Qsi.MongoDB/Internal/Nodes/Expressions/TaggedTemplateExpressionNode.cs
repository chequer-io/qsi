using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class TaggedTemplateExpressionNode : BaseNode, IExpressionNode
    {
        public IExpressionNode Tag { get; set; }
        
        public TemplateLiteralNode Quasi { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Tag;
                yield return Quasi;
            }
        }
    }
}
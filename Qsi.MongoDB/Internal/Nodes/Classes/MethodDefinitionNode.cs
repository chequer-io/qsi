using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class MethodDefinitionNode : BaseNode
    {
        public IExpressionNode Key { get; set; }
        
        public FunctionExpressionNode Value { get; set; }
        
        public string Kind { get; set; }
        
        public bool Computed { get; set; }
        
        public bool Static { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Key;
                yield return Value;
            }
        }
    }
}
using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class DoWhileStatementNode : BaseNode, IStatementNode
    {
        public IStatementNode Body { get; set; }
        
        public IExpressionNode Test { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Body;
                yield return Test;
            }
        }
    }
}
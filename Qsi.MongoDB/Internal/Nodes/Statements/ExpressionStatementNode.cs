using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class ExpressionStatementNode : BaseNode, IStatementNode
    {
        public IExpressionNode Expression { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Expression;
            }
        }
    }
}
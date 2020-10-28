using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class ForStatementNode : BaseNode, IStatementNode
    {
        // VariableDeclaration, BaseExpression
        public INode Init { get; set; }
        
        public IExpressionNode Test { get; set; }
        
        public IExpressionNode Update { get; set; }
        
        public IStatementNode Body { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Init;
                yield return Test;
                yield return Update;
                yield return Body;
            }
        }
    }
}
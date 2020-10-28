using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class TryStatementNode : BaseNode, IStatementNode
    {
        public BlockStatementNode Block { get; set; }
        
        public CatchClauseNode Handler { get; set; }
        
        public BlockStatementNode Finalizer { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Block;
                yield return Handler;
                yield return Finalizer;
            }
        }
    }
}
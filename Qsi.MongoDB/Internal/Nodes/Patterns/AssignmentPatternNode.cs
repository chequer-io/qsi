using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class AssignmentPatternNode : BaseNode, IPatternNode
    {
        public IPatternNode Left { get; set; }
        
        public IExpressionNode Right { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Left;
                yield return Right;
            }
        }
    }
}
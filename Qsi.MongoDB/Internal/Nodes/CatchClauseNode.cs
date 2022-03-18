using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class CatchClauseNode : BaseNode, INode
{
    public IPatternNode Param { get; set; }

    public BlockStatementNode Body { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Param;
            yield return Body;
        }
    }
}

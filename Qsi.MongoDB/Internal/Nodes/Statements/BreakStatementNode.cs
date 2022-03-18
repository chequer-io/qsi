using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class BreakStatementNode : BaseNode, IStatementNode
{
    public IdentifierNode Label { get; set; }

    public override IEnumerable<INode> Children
    {
        get { yield return Label; }
    }
}

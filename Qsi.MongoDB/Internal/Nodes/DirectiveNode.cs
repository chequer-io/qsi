using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class DirectiveNode : BaseNode, INode
{
    public LiteralNode Expression { get; set; }

    public string Directive { get; set; }

    public override IEnumerable<INode> Children
    {
        get { yield return Expression; }
    }
}

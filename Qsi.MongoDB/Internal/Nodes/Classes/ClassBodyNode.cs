using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class ClassBodyNode : BaseNode
{
    public MethodDefinitionNode[] Body { get; set; }

    public override IEnumerable<INode> Children => Body;
}

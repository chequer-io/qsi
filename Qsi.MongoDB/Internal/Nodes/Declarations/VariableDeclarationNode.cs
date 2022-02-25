using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class VariableDeclarationNode : BaseNode, IDeclarationNode
{
    public VariableDeclaratorNode[] Declarations { get; set; }

    // var, let, const
    public string Kind { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            foreach (var node in Declarations)
                yield return node;
        }
    }
}

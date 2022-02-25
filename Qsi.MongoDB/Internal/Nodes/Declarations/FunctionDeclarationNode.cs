using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class FunctionDeclarationNode : BaseNode, IFunctionNode, IDeclarationNode
{
    public IdentifierNode Id { get; set; }

    public IPatternNode[] Params { get; set; }

    public bool Generator { get; set; }

    public bool Async { get; set; }

    public INode Body { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            foreach (var node in Params)
                yield return node;

            yield return Body;
            yield return Id;
        }
    }
}

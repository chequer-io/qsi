using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class ClassDeclarationNode : BaseNode, IClassNode, IDeclarationNode
{
    public IdentifierNode Id { get; set; }

    public IExpressionNode SuperClass { get; set; }

    public ClassBodyNode Body { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Id;
            yield return SuperClass;
            yield return Body;
        }
    }
}

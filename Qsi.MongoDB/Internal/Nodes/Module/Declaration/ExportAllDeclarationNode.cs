using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class ExportAllDeclarationNode : BaseNode, IModuleDeclarationNode
{
    public LiteralNode Source { get; set; }

    public override IEnumerable<INode> Children
    {
        get { yield return Source; }
    }
}

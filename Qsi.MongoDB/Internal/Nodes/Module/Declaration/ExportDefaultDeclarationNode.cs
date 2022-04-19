using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class ExportDefaultDeclarationNode : BaseNode, IModuleDeclarationNode
{
    public INode Declaration { get; set; }

    public override IEnumerable<INode> Children
    {
        get { yield return Declaration; }
    }
}

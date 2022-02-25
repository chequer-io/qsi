using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class ExportSpecifierNode : IModuleSpecifierNode
{
    public IdentifierNode Exported { get; set; }

    public IEnumerable<INode> Children
    {
        get { yield return Exported; }
    }
}

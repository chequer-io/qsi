using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class ExportNamedDeclarationNode : BaseNode, IModuleDeclarationNode
    {
        public IDeclarationNode Declaration { get; set; }
        
        public ExportSpecifierNode[] Specifiers { get; set; }
        
        public LiteralNode Source { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                yield return Declaration;

                foreach (var specifier in Specifiers)
                    yield return specifier;

                yield return Source;
            }
        }
    }
}

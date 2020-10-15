using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes
{
    public class ImportDeclarationNode : BaseNode, IModuleDeclarationNode
    {
        public INode[] Specifiers { get; set; }
        
        public LiteralNode Source { get; set; }

        public override IEnumerable<INode> Children
        {
            get
            {
                foreach (var specifier in Specifiers)
                    yield return specifier;

                yield return Source;
            }
        }
    }
}
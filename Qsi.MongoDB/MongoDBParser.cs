using System.Threading;
using Qsi.Data;
using Qsi.MongoDB.Acorn;
using Qsi.MongoDB.Internal.Nodes;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.MongoDB
{
    public class MongoDBParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var node = AcornParser.GetAstNode(script.Script);

            if (node is ProgramNode programNode)
                node = programNode.Body[0];
            
            return new QsiMongoTreeNode(node);
        }
    }
}

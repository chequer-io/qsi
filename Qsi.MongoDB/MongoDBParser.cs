using System.Threading;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.MongoDB
{
    public class MongoDBParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}

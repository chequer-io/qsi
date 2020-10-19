using System.Collections.Generic;
using Qsi.MongoDB.Internal.Nodes;
using Qsi.Tree;
using Qsi.Tree.Data;

namespace Qsi.MongoDB
{
    public class MongoTreeNode : IQsiTreeNode
    {
        public IQsiTreeNode Parent => null;

        public IEnumerable<IQsiTreeNode> Children => null;

        public IUserDataHolder UserData => null;
        
        public INode Node { get; set; }

        public MongoTreeNode(INode node)
        {
            Node = node;
        }
    }
}

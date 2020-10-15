using System.Collections.Generic;
using System.Linq;
using Qsi.Diagnostics;
using Qsi.MongoDB.Internal.Nodes;

namespace Qsi.MongoDB.Diagnostics
{
    public class MongoDBRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal MongoDBRawTree(INode node)
        {
            DisplayName = node.GetType().Name;
            Children = GetChildrenByProperties(node);
        }

        internal MongoDBRawTree(string name, IEnumerable<INode> tree)
        {
            DisplayName = name;

            Children = tree.Select(t => t == null ? new MongoDBRawTree("null") : new MongoDBRawTree(t))
                .Cast<IRawTree>()
                .ToArray();
        }

        internal MongoDBRawTree(string name, object value)
        {
            DisplayName = name;

            Children = new IRawTree[]
            {
                new MongoDBRawTreeTerminalNode(value)
            };
        }

        internal MongoDBRawTree(string displayName)
        {
            DisplayName = displayName;
        }

        public static readonly string[] UnnecessaryProperties = 
            { "Start", "End", "Loc", "Range" };
        
        public static bool IsUnnecessaryProperty(string name)
        {
            return UnnecessaryProperties.Contains(name);
        }
        
        private static IRawTree[] GetChildrenByProperties(INode tree)
        {
            return tree.GetType().GetProperties()
                .Select(pi => (pi, pi.GetValue(tree)))
                .Where(x => x.Item2 != null && !IsUnnecessaryProperty(x.pi.Name))
                .Select(x =>
                {
                    var (pi, value) = x;
                    IRawTree rawTree;

                    if (typeof(INode).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new MongoDBRawTree((INode)value);
                    }
                    else if (typeof(INode[]).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new MongoDBRawTree(pi.Name, (INode[])value);
                    }
                    else
                    {
                        rawTree = new MongoDBRawTree(pi.Name, value);
                    }

                    return rawTree;
                })
                .ToArray();
        }
    }
}

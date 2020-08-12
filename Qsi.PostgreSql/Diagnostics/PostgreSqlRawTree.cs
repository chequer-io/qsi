using System.Collections.Generic;
using System.Linq;
using Qsi.Diagnostics;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10.Types;

namespace Qsi.PostgreSql.Diagnostics
{
    public sealed class PostgreSqlRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal PostgreSqlRawTree(IPgNode tree)
        {
            DisplayName = tree.GetType().Name;
            Children = GetChildrenByProperties(tree);
        }

        internal PostgreSqlRawTree(string name, IEnumerable<IPgNode> tree)
        {
            DisplayName = name;

            Children = tree.Select(t => t == null ? new PostgreSqlRawTree("null") : new PostgreSqlRawTree(t))
                .Cast<IRawTree>()
                .ToArray();
        }

        internal PostgreSqlRawTree(string name, object value)
        {
            DisplayName = name;

            Children = new IRawTree[]
            {
                new PostgreSqlRawTreeTerminalNode(value)
            };
        }

        internal PostgreSqlRawTree(string displayName)
        {
            DisplayName = displayName;
        }

        private static IRawTree[] GetChildrenByProperties(IPgNode tree)
        {
            return tree.GetType().GetProperties()
                .Select(pi => (pi, pi.GetValue(tree)))
                .Where(x => x.Item2 != null && x.pi.PropertyType != typeof(NodeTag))
                .Select(x =>
                {
                    var (pi, value) = x;
                    IRawTree rawTree;

                    if (typeof(IPgNode).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new PostgreSqlRawTree((IPgNode)value);
                    }
                    else if (typeof(IPgNode[]).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new PostgreSqlRawTree(pi.Name, (IPgNode[])value);
                    }
                    else
                    {
                        rawTree = new PostgreSqlRawTree(pi.Name, value);
                    }

                    return rawTree;
                })
                .ToArray();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Qsi.Diagnostics;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10.Types;

namespace Qsi.PostgreSql.Diagnostics
{
    public sealed class PostgreSqlLegacyRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal PostgreSqlLegacyRawTree(IPgNode tree)
        {
            DisplayName = tree.GetType().Name;
            Children = GetChildrenByProperties(tree);
        }

        internal PostgreSqlLegacyRawTree(string name, IEnumerable<IPgNode> tree)
        {
            DisplayName = name;

            Children = tree.Select(t => t == null ? new PostgreSqlLegacyRawTree("null") : new PostgreSqlLegacyRawTree(t))
                .Cast<IRawTree>()
                .ToArray();
        }

        internal PostgreSqlLegacyRawTree(string name, object value)
        {
            DisplayName = name;

            Children = new IRawTree[]
            {
                new PostgreSqlRawTreeTerminalNode(value)
            };
        }

        internal PostgreSqlLegacyRawTree(string displayName)
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
                        rawTree = new PostgreSqlLegacyRawTree((IPgNode)value);
                    }
                    else if (typeof(IPgNode[]).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new PostgreSqlLegacyRawTree(pi.Name, (IPgNode[])value);
                    }
                    else
                    {
                        rawTree = new PostgreSqlLegacyRawTree(pi.Name, value);
                    }

                    return rawTree;
                })
                .ToArray();
        }
    }
}

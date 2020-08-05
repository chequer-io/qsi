using System.Collections.Generic;
using System.Linq;
using Qsi.Diagnostics;
using Qsi.PostgreSql.Internal;

namespace Qsi.PostgreSql.Diagnostics
{
    public sealed class PostgreSqlRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal PostgreSqlRawTree(IPgTree tree)
        {
            DisplayName = tree.GetType().Name;
            Children = GetChildrenByProperties(tree);
        }

        internal PostgreSqlRawTree(string name, IEnumerable<IPgTree> tree)
        {
            DisplayName = name;

            Children = tree.Select(t => new PostgreSqlRawTree(t))
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

        private static IRawTree[] GetChildrenByProperties(IPgTree tree)
        {
            return tree.GetType().GetProperties()
                .Select(pi => (pi, pi.GetValue(tree)))
                .Where(x => x.Item2 != null)
                .Select(x =>
                {
                    var (pi, value) = x;
                    IRawTree rawTree;

                    if (typeof(IPgTree).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new PostgreSqlRawTree((IPgTree)value);
                    }
                    else if (typeof(IPgTree[]).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new PostgreSqlRawTree(pi.Name, (IPgTree[])value);
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

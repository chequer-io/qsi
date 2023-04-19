using System;
using System.Collections.Generic;
using System.Linq;
using PgQuery;
using Qsi.Diagnostics;
using Qsi.PostgreSql.Extensions;

namespace Qsi.PostgreSql.Diagnostics
{
    public sealed class PostgreSqlRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        internal PostgreSqlRawTree(Node tree)
        {
            DisplayName = tree.nodeCase.ToString();
            Children = GetChildrenByProperties(tree);
        }

        internal PostgreSqlRawTree(IPgNode tree)
        {
            DisplayName = tree.GetType().Name;
            Children = GetChildrenByProperties(tree.ToNode());
        }

        internal PostgreSqlRawTree(string name, IEnumerable<IPgNode> tree)
        {
            DisplayName = name;

            Children = tree.Select(t => t.ToNode())
                .Select(t => new PostgreSqlRawTree(t))
                .Cast<IRawTree>()
                .ToArray();
        }

        internal PostgreSqlRawTree(string name, IEnumerable<Node> tree)
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

        internal PostgreSqlRawTree(string displayName)
        {
            DisplayName = displayName;
        }

        private static IRawTree[] GetChildrenByProperties(Node tree)
        {
            var nodeType = tree.nodeCase;

            if (nodeType is Node.nodeOneofCase.None)
                return Array.Empty<IRawTree>();

            var value = tree.GetType().GetProperties()
                .Select(pi => (pi, pi.GetValue(tree)))
                .Where(x => x.Item2 is not null && x.pi.Name == nodeType.ToString())
                .Select(x => x.Item2)
                .FirstOrDefault()!;

            return value.GetType().GetProperties()
                .Where(x => !x.PropertyType.Name.EndsWith("OneofCase") &&
                            x.Name != "Location")
                .Select(x => (x, x.GetValue(value)))
                .Where(x => x.Item2 is not null)
                .Select(x =>
                {
                    var (pi, v) = x;
                    IRawTree rawTree;

                    if (typeof(IPgNode).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new PostgreSqlRawTree(((IPgNode)v!).ToNode());
                    }
                    else if (typeof(IEnumerable<IPgNode>).IsAssignableFrom(pi.PropertyType))
                    {
                        if (v is List<IPgNode> { Count: 0 })
                            return null;

                        rawTree = new PostgreSqlRawTree(pi.Name, (IEnumerable<IPgNode>)v!);
                    }
                    else if (typeof(Node).IsAssignableFrom(pi.PropertyType))
                    {
                        rawTree = new PostgreSqlRawTree((Node)v!);
                    }
                    else if (typeof(IEnumerable<Node>).IsAssignableFrom(pi.PropertyType))
                    {
                        if (v is List<Node> { Count: 0 })
                            return null;

                        rawTree = new PostgreSqlRawTree(pi.Name, (IEnumerable<Node>)v!);
                    }
                    else
                    {
                        if (IsDefault(v))
                            return null;

                        rawTree = new PostgreSqlRawTreeTerminalNode(pi.Name, v);
                    }

                    return rawTree;
                })
                .WhereNotNull()
                .ToArray();
        }

        private static bool IsDefault(object? value)
        {
            return value is null or false or 0 or 0L or 0F or 0D or "";
        }
    }
}

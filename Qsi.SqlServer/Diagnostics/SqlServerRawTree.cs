using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Diagnostics;

namespace Qsi.SqlServer.Diagnostics
{
    internal sealed class SqlServerRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; private set; }

        public int ChildrenCount => _children?.Count ?? Children.Length;

        private List<IRawTree> _children;

        public SqlServerRawTree(string displayName)
        {
            DisplayName = displayName;
            _children = new List<IRawTree>();
        }

        public SqlServerRawTree(SqlCodeObject tree)
        {
            DisplayName = tree.GetType().Name;
            SqlCodeObject[] childrens = tree.Children.ToArray();
            int count = childrens.Length;

            if (childrens.Length == 0)
            {
                Children = new IRawTree[] { new SqlServerRawTreeTerminalNode(tree.Sql) };
            }
            else
            {
                var trees = new IRawTree[count];

                for (int i = 0; i < count; i++)
                {
                    var child = childrens[i];
                    trees[i] = new SqlServerRawTree(child);
                }

                Children = trees;
            }
        }

        internal void AddChild(IRawTree rawTree)
        {
            _children.Add(rawTree);
        }

        internal void Freeze()
        {
            if (Children != null)
                throw new InvalidOperationException();

            Children = _children.ToArray();
            _children = null;
        }
    }
}

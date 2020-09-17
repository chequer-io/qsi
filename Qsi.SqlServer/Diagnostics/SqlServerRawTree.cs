using System;
using System.Collections.Generic;
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

using System;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;

namespace Qsi.Diagnostics.Antlr
{
    public readonly struct AntlrTreeTree : IRawTree
    {
        public Type TreeType => _tree.GetType();

        public IEnumerable<IRawTree> Children { get; }

        private readonly ITree _tree;

        public AntlrTreeTree(ITree tree)
        {
            _tree = tree;

            int count = _tree.ChildCount;
            var children = new IRawTree[count];

            for (int i = 0; i < count; i++)
                children[i] = new AntlrTreeTree(_tree.GetChild(i));

            Children = children;
        }
    }
}

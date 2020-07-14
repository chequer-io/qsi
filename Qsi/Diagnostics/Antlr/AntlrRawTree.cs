using Antlr4.Runtime.Tree;

namespace Qsi.Diagnostics.Antlr
{
    public readonly struct AntlrRawTree : IRawTree
    {
        public string DisplayName { get; }

        public IRawTree[] Children { get; }

        public AntlrRawTree(ITree tree)
        {
            int count = tree.ChildCount;
            var children = new IRawTree[count];

            for (int i = 0; i < count; i++)
            {
                var child = tree.GetChild(i);

                if (child is ITerminalNode terminalNode)
                    children[i] = new AntlrRawTreeTerminalNode(terminalNode);
                else
                    children[i] = new AntlrRawTree(child);
            }

            Children = children;
            DisplayName = tree.GetType().Name;

            if (tree is ITerminalNode)
                DisplayName += $": {tree}";
        }
    }
}

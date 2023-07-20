using Antlr4.Runtime.Tree;

namespace Qsi.Diagnostics.Antlr;

public class AntlrRawTree : IRawTree
{
    public string DisplayName { get; }

    public IRawTree[] Children { get; }

    public AntlrRawTree(ITree tree, string[] ruleNames)
    {
        int count = tree.ChildCount;
        var children = new IRawTree[count];

        for (int i = 0; i < count; i++)
        {
            var child = tree.GetChild(i);

            if (child is ITerminalNode terminalNode)
                children[i] = new AntlrRawTreeTerminalNode(terminalNode);
            else
                children[i] = new AntlrRawTree(child, ruleNames);
        }

        Children = children;

        if (tree is IRuleNode ruleNode)
        {
            DisplayName = ruleNames[ruleNode.RuleContext.RuleIndex];
        }
        else
        {
            DisplayName = tree.GetType().Name;
        }

        if (tree is ITerminalNode)
            DisplayName += $": {tree}";
    }
}
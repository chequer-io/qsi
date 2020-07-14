using Antlr4.Runtime.Tree;

namespace Qsi.Diagnostics.Antlr
{
    public abstract class AntlrRawParserBase : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            (var tree, string[] ruleNames) = ParseAntlrTree(input);

            return new AntlrRawTree(tree, ruleNames);
        }

        protected abstract (ITree Tree, string[] RuleNames) ParseAntlrTree(string input);
    }
}

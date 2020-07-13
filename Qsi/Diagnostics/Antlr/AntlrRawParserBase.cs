using Antlr4.Runtime.Tree;

namespace Qsi.Diagnostics.Antlr
{
    public abstract class AntlrRawParserBase : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            return new AntlrTreeTree(ParseAntlrTree(input));            
        }

        protected abstract ITree ParseAntlrTree(string input);
    }
}

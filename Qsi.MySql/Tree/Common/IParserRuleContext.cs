using Antlr4.Runtime;

namespace Qsi.MySql.Tree.Common
{
    internal interface IParserRuleContext
    {
        IToken Start { get; }

        IToken Stop { get; }
    }
}

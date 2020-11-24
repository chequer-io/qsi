using Antlr4.Runtime;

namespace Qsi.Cql.Tree.Common
{
    internal interface IParserRuleContext
    {
        IToken Start { get; }

        IToken Stop { get; }
    }
}

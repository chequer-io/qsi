using Antlr4.Runtime;

namespace Qsi.Shared;

internal interface IParserRuleContext
{
    IToken Start { get; }

    IToken Stop { get; }
}
using Antlr4.Runtime;

namespace Qsi.MySql.Tree.Common
{
    internal interface ICommonContext
    {
        IToken Start { get; }

        IToken Stop { get; }
    }
}

using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;

namespace Qsi.Parsing
{
    public interface IQsiParser
    {
        IParseTree ParseTree(ICharStream stream);

        IEnumerable<QsiScript> ParseScripts(string script);
    }
}

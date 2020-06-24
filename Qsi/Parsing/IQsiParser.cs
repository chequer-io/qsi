using System;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Parsing
{
    public interface IQsiParser
    {
        event EventHandler<QsiSyntaxErrorException> SyntaxError;

        IQsiTreeNode ParseTree(QsiScript script);

        IEnumerable<QsiScript> ParseScripts(string script);
    }
}

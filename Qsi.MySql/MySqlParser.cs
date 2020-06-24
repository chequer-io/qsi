using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.MySql.Internal;
using Qsi.Parsing.Antlr;
using Qsi.Tree;

namespace Qsi.MySql
{
    public sealed class MySqlParser : AntlrParserBase
    {
        protected override Parser CreateParser(QsiScript script)
        {
            var stream = new AntlrUpperInputStream(script.Script);
            var lexer = new MySqlLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            return new Internal.MySqlParser(tokens);
        }

        protected override IQsiTreeNode ParseTree(QsiScript script, Parser parser)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<QsiScript> ParseScripts(string script)
        {
            throw new NotImplementedException();
        }
    }
}

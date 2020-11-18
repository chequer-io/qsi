using System;
using Antlr4.Runtime;
using Qsi.Cassandra.Internal;
using Qsi.Data;
using Qsi.Parsing.Antlr;
using Qsi.Tree;

namespace Qsi.Cassandra
{
    public sealed class CqlParser : AntlrParserBase
    {
        protected override Parser CreateParser(QsiScript script)
        {
            var stream = new AntlrInputStream(script.Script);
            var lexer = new CqlLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            return new Internal.CqlParser(tokens);
        }

        protected override IQsiTreeNode Parse(QsiScript script, Parser parser)
        {
            throw new NotImplementedException();
        }
    }
}

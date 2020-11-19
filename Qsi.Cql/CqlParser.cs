using System;
using System.Threading;
using Antlr4.Runtime;
using Qsi.Cql.Internal;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.Cql
{
    public sealed class CqlParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var stream = new AntlrInputStream(script.Script);
            var lexer = new CqlLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CqlParserInternal(tokens);
            parser.AddErrorListener(new ErrorListener());

            throw new NotImplementedException();
        }
    }
}

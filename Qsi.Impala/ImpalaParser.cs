using System;
using System.Threading;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Impala.Internal;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.Impala
{
    public sealed class ImpalaParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var stream = new AntlrInputStream(script.Script);
            var lexer = new ImpalaLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ImpalaParserInternal(tokens);
            parser.AddErrorListener(new ErrorListener());

            throw new NotImplementedException();
        }
    }
}

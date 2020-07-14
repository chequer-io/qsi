using System;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Parsing.Antlr;
using Qsi.PostgreSql.Internal;
using Qsi.Tree;

namespace Qsi.PostgreSql
{
    public class PostgreSqlParser : AntlrParserBase
    {
        protected override Parser CreateParser(QsiScript script)
        {
            var stream = new AntlrUpperInputStream(script.Script);
            var lexer = new PlSqlLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            return new PlSqlParser(tokens);
        }

        protected override IQsiTreeNode Parse(QsiScript script, Parser parser)
        {
            throw new NotImplementedException();
        }
    }
}

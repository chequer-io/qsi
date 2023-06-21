using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrimarSql;
using PrimarSql.Internal;
using Qsi.Diagnostics.Antlr;

namespace Qsi.PrimarSql.Diagnostics;

public class PrimarSqlRawParser : AntlrRawParserBase
{
    protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
    {
        var stream = new AntlrUpperInputStream(input);
        var lexer = new PrimarSqlLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new global::PrimarSql.Internal.PrimarSqlParser(tokens);

        return (parser.root(), parser.RuleNames);
    }
}
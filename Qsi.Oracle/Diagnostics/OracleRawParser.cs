using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Diagnostics.Antlr;
using Qsi.Oracle.Internal;

namespace Qsi.Oracle.Diagnostics;

public class OracleRawParser : AntlrRawParserBase
{
    protected override (ITree Tree, string[] RuleNames) ParseAntlrTree(string input)
    {
        var stream = new AntlrInputStream(input);
        var lexer = new OracleLexerInternal(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new OracleParserInternal(tokens);
        parser.AddErrorListener(new ErrorListener());

        return (parser.root(), parser.RuleNames);
    }
}
using System.Linq;
using NUnit.Framework;
using Qsi.Oracle;
using Qsi.Parsing.Common;
using static Qsi.Parsing.Common.CommonScriptParser;

namespace Qsi.Tests.Oracle;

public partial class OracleScriptParserTest
{
    [TestCaseSource(nameof(Parse_TestDatas))]
    public string[] Test_Parse(string sql)
    {
        var parser = new OracleScriptParser();

        return parser.Parse(sql, default)
            .Select(x => x.Script)
            .ToArray();
    }

    [TestCaseSource(nameof(ParseTokens_TestDatas))]
    public string[] Test_ParseTokens(string sql)
    {
        var parser = new OracleScriptParser();

        return parser.ParseTokens(new CommonScriptCursor(sql))
            .Where(t => t.Type is not (TokenType.WhiteSpace or TokenType.SingeLineComment or TokenType.MultiLineComment))
            .Select(x => sql[x.Span])
            .ToArray();
    }
}

using System.Linq;
using NUnit.Framework;
using Qsi.Oracle;

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
}

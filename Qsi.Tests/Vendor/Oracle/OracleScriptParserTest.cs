using System.Linq;
using NUnit.Framework;
using Qsi.Data;
using Qsi.Oracle;

namespace Qsi.Tests.Oracle;

public partial class OracleScriptParserTest
{
    [TestCaseSource(nameof(Parse_TestDatas))]
    public string Test_Parse(string sql)
    {
        var parser = new OracleScriptParser();
        QsiScript[] scripts = parser.Parse(sql, default).ToArray();

        Assert.AreEqual(1, scripts.Length);

        return scripts[0].Script;
    }
}

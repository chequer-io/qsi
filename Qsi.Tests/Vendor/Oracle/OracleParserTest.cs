using NUnit.Framework;
using Qsi.Data;
using Qsi.Oracle;

namespace Qsi.Tests.Oracle;

public partial class OracleParserTest
{
    [TestCaseSource(nameof(Parse_TestDatas))]
    public void Test_Parse(string sql)
    {
        var parser = new OracleParser();

        parser.Parse(new QsiScript(sql, QsiScriptType.Unknown));
    }
}

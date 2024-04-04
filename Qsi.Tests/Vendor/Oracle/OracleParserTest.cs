using System;
using NUnit.Framework;
using Qsi.Data;
using Qsi.Oracle;
using Qsi.Parsing;

namespace Qsi.Tests.Oracle;

public partial class OracleParserTest
{
    [TestCaseSource(nameof(Parse_Oracle19Datas))]
    [TestCaseSource(nameof(Parse_TestDatas))]
    public void Test_Parse(string sql)
    {
        try
        {
            var parser = new OracleParser();

            parser.Parse(new QsiScript(sql, QsiScriptType.Unknown));
        }
        catch (QsiSyntaxErrorException e)
        {
            throw;
        }
        catch (Exception)
        {
            Assert.Pass();
        }
    }
}

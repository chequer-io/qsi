using NUnit.Framework;
using Qsi.Data;
using Qsi.PostgreSql.Data;

namespace Qsi.Tests.Vendor.PostgreSQL.Data;

public class PostgreSqlStringTest
{
    [Timeout(1000)]
    [TestCase(PostgreSqlStringKind.National, "national", ExpectedResult = "N'national'")]
    [TestCase(PostgreSqlStringKind.CharString, "foobar", ExpectedResult = "CHAR'foobar'")]
    [TestCase(PostgreSqlStringKind.NCharString, "foobar", ExpectedResult = "NCHAR'foobar'")]
    [TestCase(PostgreSqlStringKind.BpCharString, "foobar", ExpectedResult = "BPCHAR'foobar'")]
    [TestCase(PostgreSqlStringKind.VarcharString, "foobar", ExpectedResult = "VARCHAR'foobar'")]
    [TestCase(PostgreSqlStringKind.Hex, "4f2", ExpectedResult = "X'4f2'")]
    [TestCase(PostgreSqlStringKind.Bin, "011001", ExpectedResult = "B'011001'")]
    [TestCase(PostgreSqlStringKind.CharString, "foo'bar", ExpectedResult = "CHAR'foo''bar'")]
    public string Test_ToString(PostgreSqlStringKind kind, string input)
    {
        var postgreSqlString = new PostgreSqlString(kind, input);
        
        return postgreSqlString.ToString();
    }

    [Timeout(1000)]
    [TestCase("N'national'", PostgreSqlStringKind.National, "national")]
    [TestCase("CHAR'foobar'", PostgreSqlStringKind.CharString, "foobar")]
    [TestCase("NCHAR'foobar'", PostgreSqlStringKind.NCharString, "foobar")]
    [TestCase("BPCHAR'foobar'", PostgreSqlStringKind.BpCharString, "foobar")]
    [TestCase("VARCHAR'foobar'", PostgreSqlStringKind.VarcharString, "foobar")]
    [TestCase("X'0F'", PostgreSqlStringKind.Hex, "0F")]
    [TestCase("B'01'", PostgreSqlStringKind.Bin, "01")]
    [TestCase("cHaR'foobar'", PostgreSqlStringKind.CharString, "foobar")]
    [TestCase("char 'woobar'", PostgreSqlStringKind.CharString, "woobar")]
    [TestCase("char 'woo''bar'", PostgreSqlStringKind.CharString, "woo'bar")]
    public void Test_UnescapedValueConstructor(string unescaped, PostgreSqlStringKind kind, string result)
    {
        var postgreSqlString = new PostgreSqlString(unescaped);
        Assert.AreEqual(kind, postgreSqlString.Kind);
        Assert.AreEqual(QsiDataType.String, postgreSqlString.Type);
        Assert.AreEqual(result, postgreSqlString.Value);
    }
}

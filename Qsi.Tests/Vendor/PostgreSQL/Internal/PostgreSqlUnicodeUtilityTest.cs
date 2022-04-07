using System;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qsi.PostgreSql.Internal;

namespace Qsi.Tests.Vendor.PostgreSQL.Internal;

[TestFixture]
public class PostgreSqlUnicodeUtilityTest
{
    [Timeout(1000)]
    [TestCase("U&\"d\\0061t\\0061\"", ExpectedResult = "\"data\"")]
    [TestCase("U&\"d!0061 t!+000061\" UESCAPE '!'", ExpectedResult = "\"da ta\"")]
    
    // TODO: Seperate test cases with errors.
    [TestCase("u&\"da0061taa a+000061 a aa aa\" UESCAPE 'a'", ExpectedResult = "")]
    public string Test_GetString(string input)
    {
        var result = PostgreSqlUnicodeUtility.GetString(input);

        return result;
    }
    
    [Timeout(1000)]
    [TestCase("U&\"d\\0061t\\0061\"", ExpectedResult = "d\\0061t\\0061")]
    [TestCase("U&\"d\\0061t\\0061", ExpectedResult = "\"d\\0061t\\0061")]
    public string Test_Dequote(string quotedInput)
    {
        var result = PostgreSqlUnicodeUtility.Dequote(quotedInput);

        return result;
    }
    
    [Timeout(1000)]
    [TestCase("d\\0061t\\0061", '\\', ExpectedResult = "data")]
    [TestCase("d!0061t!+000061", '!', ExpectedResult = "data")]
    [TestCase("d!0061t!+000061!!", '!', ExpectedResult = "data!")]
    [TestCase("d\\0061t\\+000061 y\\y", '\\', ExpectedResult = "data y\\y")]
    
    // TODO: Seperate test cases with errors.
    [TestCase("d\\", '\\', ExpectedResult = "")]
    [TestCase("\\\\ \\        ", '\\', ExpectedResult = "")]
    [TestCase("da0061taa a+000061 a aa aa", 'a', ExpectedResult = "")]
    public string Test_Parse(string input, char escape)
    {
        var result = PostgreSqlUnicodeUtility.Parse(input, escape, 0);
        return result;
    }
}

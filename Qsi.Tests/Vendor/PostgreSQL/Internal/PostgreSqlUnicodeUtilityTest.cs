using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qsi.PostgreSql.Internal;

namespace Qsi.Tests.Vendor.PostgreSQL.Internal;

[TestFixture]
public class PostgreSqlUnicodeUtilityTest
{
    [TestCase("d\\0061t\\0061", '\\', ExpectedResult = "data")]
    [TestCase("d!0061t!+000061", '!', ExpectedResult = "data")]
    public string Test_ProcessUnicodeEscape(string input, char escape)
    {
        var result = PostgreSqlUnicodeUtility.ProcessUnicodeEscape(input, escape, 0);

        return result;
    }
}

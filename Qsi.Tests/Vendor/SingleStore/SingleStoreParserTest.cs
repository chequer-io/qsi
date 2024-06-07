using NUnit.Framework;
using Qsi.Parsing;
using Qsi.SingleStore.Internal;

namespace Qsi.Tests.SingleStore;

public sealed partial class SingleStoreParserTest
{
    [TestCaseSource(nameof(GetAllValidQueryTestCaseDatas))]
    public void Parse_ValidQuery_ShouldPass(string query)
    {
        var context = GetContext(query);

        if (context.simpleStatement().Length == 0)
            Assert.Fail("Failed to ");

        Assert.Pass();
    }

    [TestCase("4gmvrt3ygx")]
    public void Parse_InvalidQuery_ShouldFail(string query)
    {
        try
        {
            var context = GetContext(query);

            if (context.simpleStatement().Length == 0)
                Assert.Pass();

            Assert.Fail();
        }
        catch (QsiSyntaxErrorException syntax)
        {
            Assert.Pass(syntax.Message);
        }
    }

    private SingleStoreParserInternal.QueryContext GetContext(string query)
    {
        var parser = SingleStoreUtility.CreateParser(query);
        var context = parser.query();

        return context;
    }
}

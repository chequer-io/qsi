using NUnit.Framework;
using Qsi.Tests.Vendor.PostgreSql.Utilities;

namespace Qsi.Tests.PostgreSql;

public class PostgreSqlTestQueryImporterTest
{

    [TestCase(
        "Resources/links.test.json",
        "Resources/queries.test.json",
        "Resources/custom_queries.test.json",
        51)]
    [TestCase(
        "Resources/links.test.json",
        "Resources/queries.test.json", 
        null,
        43)]
    public void Import_OK(
        string linkPath, 
        string queryPath,
        string customQueryPath,
        int queryLength)
    {
        var importer = new PostgreSqlTestQueryImporter(linkPath, queryPath, customQueryPath);

        var queries = importer.Import();
        
        Assert.AreEqual(queryLength, queries.Length);
        
        foreach (var query in queries)
        {
            if(string.IsNullOrEmpty(query))
                Assert.Fail("Query should not be empty.");
        }
    }

    [TestCase(
        "Resources/links.test.json",
        "there-is-no-such-path-like-this.json",
        null)]
    [TestCase(
        "there-is-no-such-path-like-this.json",
        "Resources/queries.test.json",
        null)]
    public void Import_Fail_When_File_Not_Found(
        string linkPath,
        string queryPath,
        string customQueryPath)
    {
        var importer = new PostgreSqlTestQueryImporter(linkPath, queryPath, customQueryPath);

        Assert.Catch(() => importer.Import());
    }
}

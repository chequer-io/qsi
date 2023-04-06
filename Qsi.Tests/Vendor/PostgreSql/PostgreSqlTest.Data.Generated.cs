using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Qsi.Tests.Vendor.PostgreSql.Utilities;

namespace Qsi.Tests.PostgreSql;

public partial class PostgreSqlTest
{
    private const string _linkPath = "Resources/links.10.json";
    private const string _queryPath = "Resources/queries.10.json";
    private const string _customQueryPath = "Resources/custom_queries.10.json";

    private const string _datagripPath = "Resources/datagrip_queries.test.csv";
    
    private static readonly PostgreSqlTestQueryImporter _importer = new(_linkPath, _queryPath, _customQueryPath);
    private static readonly CsvPostgreSqlTestQueryImporter _csvImporter = new(_datagripPath);

    private static TestCaseData[] _pgTestCaseDatas = TestCaseDatas(_importer).ToArray();
    private static TestCaseData[] _dataGripTestDatas = TestCaseDatas(_csvImporter).ToArray();

    public static IEnumerable<TestCaseData> GetTestCaseDatas(string path)
    {
        var importer = new JsonPostgreSqlTestQueryImporter(path);

        var queries = importer.Import();
        var testCaseDatas = queries.Select(q => new TestCaseData(q));

        return testCaseDatas;
    }
    
    private static IEnumerable<TestCaseData> TestCaseDatas(IPostgreSqlTestQueryImporter importer)
    {
        var queries = importer.Import();
        IEnumerable<TestCaseData> testCaseDatas = queries.Select(q => new TestCaseData(q));

        return testCaseDatas;
    }
}
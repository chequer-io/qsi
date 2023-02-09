using System;
using NUnit.Framework;

namespace Qsi.Tests.PostgreSql;

public class CqlPostgreTestQueryImporterTest
{
    [TestCase("Resources/datagrip_queries.test.csv")]
    public void Import_OK(string path)
    {
        var importer = new CsvPostgreSqlTestQueryImporter(path);
        var queries = importer.Import();

        foreach (var query in queries)
        {
            Console.WriteLine(query);
            Console.WriteLine();
        }
    }
}

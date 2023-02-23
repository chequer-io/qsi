using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Qsi.Tests.Vendor.PostgreSql.Utilities;

namespace Qsi.Tests.PostgreSql;

public class CsvPostgreSqlTestQueryImporter : IPostgreSqlTestQueryImporter
{
    private readonly string _path;

    public CsvPostgreSqlTestQueryImporter(string path)
    {
        _path = path;
    }
    
    public string[] Import()
    {
        using var streamReader = new StreamReader(_path);
        using var reader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

        return GetData(reader).ToArray();
    }

    private IEnumerable<string> GetData(CsvReader reader)
    {
        while (reader.Read())
        {
            var record = reader.GetRecord(new { Query = string.Empty });
            yield return record.Query;
        }
    }
}

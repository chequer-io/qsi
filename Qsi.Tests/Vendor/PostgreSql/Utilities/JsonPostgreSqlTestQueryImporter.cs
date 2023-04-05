using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Qsi.Tests.Vendor.PostgreSql.Utilities;

public class JsonPostgreSqlTestQueryImporter : IPostgreSqlTestQueryImporter
{
    private readonly string _queryPath;

    public JsonPostgreSqlTestQueryImporter(string queryPath)
    {
        _queryPath = queryPath;
    }
    
    public string[] Import()
    {
        using var stream = new FileStream(_queryPath, FileMode.Open);

        var queries = JsonSerializer
            .Deserialize<IEnumerable<QueryWrapper>>(stream)
            .Select(q => q.Query)
            .ToArray();

        return queries;
    }

    public class QueryWrapper
    {
        public string Query { get; set; }
    }
}

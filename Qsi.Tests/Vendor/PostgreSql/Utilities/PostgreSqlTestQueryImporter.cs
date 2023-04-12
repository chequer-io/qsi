using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Qsi.Tests.Vendor.PostgreSql.Utilities;

internal sealed class PostgreSqlTestQueryImporter : IPostgreSqlTestQueryImporter
{
    private readonly string _linkPath;
    private readonly string _queryPath;
    private readonly string _customQueryPath;

    public PostgreSqlTestQueryImporter(string linkPath, string queryPath, string customQueryPath)
    {
        _linkPath = linkPath;
        _queryPath = queryPath;
        _customQueryPath = customQueryPath;
    }

    public string[] Import()
    {
        IEnumerable<string> queries = ReadQueryFile(_linkPath, _queryPath, _customQueryPath);

        return queries.ToArray();
    }

    private static IEnumerable<string> ReadQueryFile(string linkPath, string queryPath, string customQueryPath)
    {
        var links = ReadLinkFile(linkPath);

        TryReadQueryFile(customQueryPath, out Dictionary<string, string[]> customQueries);
        Dictionary<string, string[]> queries = ReadQueryFile(queryPath);

        foreach (var link in links)
        {
            if (customQueries is not null
                && customQueries.TryGetValue(link, out var customQueryList)
                && customQueryList.Length > 0)
            {
                foreach (var query in customQueryList)
                    yield return query;
            }

            if (queries.TryGetValue(link, out var queryList) && queryList.Length > 0)
            {
                foreach (var query in queryList)
                    yield return query;
            }
        }
    }

    private static bool TryReadQueryFile(string path, out Dictionary<string, string[]> queries)
    {
        queries = null;

        try
        {
            queries = ReadQueryFile(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static Dictionary<string, string[]> ReadQueryFile(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);

        var queries = JsonSerializer.Deserialize<Dictionary<string, string[]>>(stream);
        return queries;
    }

    private static string[] ReadLinkFile(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);

        var links = JsonSerializer.Deserialize<string[]>(stream);

        return links;
    }
}

using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using Qsi.MySql;
using Qsi.Services;

namespace Qsi.Tests.Vendor.MySql.Driver;

public class MySqlLanguageService : MySqlLanguageServiceBase
{
    public override Version Version { get; }

    private readonly DbConnection _connection;

    public MySqlLanguageService(DbConnection connection)
    {
        var match = Regex.Match(connection.ServerVersion ?? string.Empty, @"\d+(?:\.\d+){1,3}");

        if (match.Success)
        {
            if (!Version.TryParse(match.Value, out var version))
                throw new Exception($"Invalid server version '{connection.ServerVersion}'");

            Version = version;
        }
        else
        {
            Version = new Version(5, 7, 13);
        }

        _connection = connection;
    }

    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new MySqlRepositoryProvider(_connection);
    }
}

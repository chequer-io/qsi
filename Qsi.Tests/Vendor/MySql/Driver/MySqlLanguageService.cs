using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using Qsi.MySql;
using Qsi.Services;

namespace Qsi.Tests.Vendor.MySql.Driver;

public class MySqlLanguageService : MySqlLanguageServiceBase
{
    public override Version Version { get; }

    public override bool MariaDBCompatibility { get; }

    private readonly DbConnection _connection;

    public MySqlLanguageService(DbConnection connection, bool mariaDBCompatibility)
    {
        var match = Regex.Match(connection.ServerVersion ?? string.Empty, @"\d+(?:\.\d+){1,3}");

        if (match.Success)
        {
            Version = Version.Parse(match.Value);
        }
        else
        {
            throw new Exception($"Invalid server version '{connection.ServerVersion}'");
        }

        _connection = connection;
        MariaDBCompatibility = mariaDBCompatibility;
    }

    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new MySqlRepositoryProvider(_connection);
    }
}

using System;
using System.Data.Common;
using Qsi.MySql;
using Qsi.Services;

namespace Qsi.Tests.Drivers.MySql;

public class MySqlLanguageService : MySqlLanguageServiceBase
{
    public override Version Version { get; }

    private readonly DbConnection _connection;

    public MySqlLanguageService(DbConnection connection)
    {
        Version = Version.Parse(connection.ServerVersion!);
        _connection = connection;
    }

    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new MySqlRepositoryProvider(_connection);
    }
}

using System.Data.Common;
using Qsi.PostgreSql;
using Qsi.Services;

namespace Qsi.Tests.PostgreSql.Driver;

public class PostgreSqlLanguageService : PostgreSqlLanguageServiceBase
{
    private readonly DbConnection _connection;
    
    public PostgreSqlLanguageService(DbConnection connection)
    {
        _connection = connection;
    }
    
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new PostgreSqlRepositoryProvider(_connection);
    }
}

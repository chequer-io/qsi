using System.Data.Common;
using Qsi.PrimarSql;
using Qsi.Services;

namespace Qsi.Tests.Vendor.PostgreSQL.Driver;

public class PrimarSqlLanguageService : PrimarSqlLanguageServiceBase
{
    private readonly DbConnection _connection;
    
    public PrimarSqlLanguageService(DbConnection connection)
    {
        _connection = connection;
    }
    
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new PrimarSqlRepositoryProvider(_connection);
    }
}

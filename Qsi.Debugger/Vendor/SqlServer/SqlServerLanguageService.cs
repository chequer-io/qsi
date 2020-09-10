using Microsoft.SqlServer.Management.SqlParser.Common;
using Qsi.Services;
using Qsi.SqlServer;

namespace Qsi.Debugger.Vendor.SqlServer
{
    public class SqlServerLanguageService : SqlServerLanguageServiceBase
    {
        public SqlServerLanguageService(DatabaseCompatibilityLevel compatibilityLevel, TransactSqlVersion transactSqlVersion) : base(compatibilityLevel, transactSqlVersion)
        {
        }

        public override IQsiReferenceResolver CreateResolver()
        {
            return new SqlServerReferenceResolver();
        }
    }
}

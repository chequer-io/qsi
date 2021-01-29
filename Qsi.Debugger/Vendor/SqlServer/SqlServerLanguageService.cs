using Qsi.Parsing;
using Qsi.Services;
using Qsi.SqlServer;
using Qsi.SqlServer.Common;

namespace Qsi.Debugger.Vendor.SqlServer
{
    public class SqlServerLanguageService : SqlServerLanguageServiceBase
    {
        public SqlServerLanguageService(TransactSqlVersion transactSqlVersion) : base(transactSqlVersion)
        {
        }

        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new SqlServerRepositoryProvider();
        }
    }
}

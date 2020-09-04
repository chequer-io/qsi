using Qsi.Services;
using Qsi.SqlServer;

namespace Qsi.Debugger.Vendor.SqlServer
{
    public class SqlServerLanguageService : SqlServerLanguageServiceBase
    {
        public override IQsiReferenceResolver CreateResolver()
        {
            return new SqlServerReferenceResolver();
        }
    }
}

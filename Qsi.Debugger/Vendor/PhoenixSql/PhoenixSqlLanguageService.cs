using Qsi.PhoenixSql;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PhoenixSql
{
    public class PhoenixSqlLanguageService : PhoenixSqlLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new PhoenixSqlRepositoryProvider();
        }
    }
}

using Qsi.PrimarSql;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PrimarSql
{
    internal class PrimarSqlLanguageService : PrimarSqlLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new PrimarSqlRepositoryProvider();
        }
    }
}

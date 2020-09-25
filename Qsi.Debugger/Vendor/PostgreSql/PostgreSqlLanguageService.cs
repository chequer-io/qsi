using Qsi.PostgreSql;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PostgreSql
{
    internal class PostgreSqlLanguageService : PostgreSqlLanguageServiceBase
    {
        public override IQsiReferenceResolver CreateReferenceResolver()
        {
            return new PostgreSqlReferenceResolver();
        }
    }
}

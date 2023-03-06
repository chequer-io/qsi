using Qsi.Diagnostics;
using Qsi.PostgreSql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PostgreSql
{
    internal class PostgreSqlLegacyDebugger : VendorDebugger
    {
        protected override IQsiLanguageService CreateLanguageService()
        {
            return new PostgreSqlLegacyLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new PostgreSqlLegacyRawParser();
        }
    }
}

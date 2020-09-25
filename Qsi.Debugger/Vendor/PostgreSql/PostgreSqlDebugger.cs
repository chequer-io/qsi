using Qsi.Diagnostics;
using Qsi.PostgreSql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PostgreSql
{
    internal class PostgreSqlDebugger : VendorDebugger
    {
        public PostgreSqlDebugger()
        {
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new PostgreSqlLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new PostgreSqlRawParser();
        }
    }
}

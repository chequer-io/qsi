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

        public override IQsiLanguageService CreateLanguageService()
        {
            return new PostgreSqlLanguageService();
        }

        public override IRawTreeParser CreateRawTreeParser()
        {
            return new PostgreSqlRawParser();
        }
    }
}

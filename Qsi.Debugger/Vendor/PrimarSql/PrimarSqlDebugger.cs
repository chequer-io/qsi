using Qsi.Diagnostics;
using Qsi.PrimarSql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PrimarSql
{
    internal class PrimarSqlDebugger : VendorDebugger
    {
        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new PrimarSqlRawParser();
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new PrimarSqlLanguageService();
        }
    }
}

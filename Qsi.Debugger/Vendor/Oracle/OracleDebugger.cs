using Qsi.Diagnostics;
using Qsi.JSql.Diagnostics;
using Qsi.Oracle.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Oracle
{
    internal sealed class OracleDebugger : VendorDebugger
    {
        protected override IQsiLanguageService CreateLanguageService()
        {
            return new OracleLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new OracleRawParser();
        }
    }
}

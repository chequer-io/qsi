using Qsi.Diagnostics;
using Qsi.Oracle.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Oracle;

internal class OracleDebugger : VendorDebugger
{
    protected override IRawTreeParser CreateRawTreeParser()
    {
        return new OracleRawParser();
    }

    protected override IQsiLanguageService CreateLanguageService()
    {
        return new OracleLanguageService();
    }
}
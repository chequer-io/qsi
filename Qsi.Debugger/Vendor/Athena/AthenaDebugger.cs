using Qsi.Athena.Diagnostics;
using Qsi.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Athena
{
    internal class AthenaDebugger : VendorDebugger
    {
        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new AthenaRawParser();
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new AthenaLanguageService();
        }
    }
}

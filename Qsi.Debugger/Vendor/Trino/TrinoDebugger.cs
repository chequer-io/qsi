using Qsi.Diagnostics;
using Qsi.Services;
using Qsi.Trino.Diagnostics;

namespace Qsi.Debugger.Vendor.Trino
{
    internal class TrinoDebugger : VendorDebugger
    {
        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new TrinoRawParser();
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new TrinoLanguageService();
        }
    }
}

using Qsi.Diagnostics;
using Qsi.Hana.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Hana
{
    internal class HanaDebugger : VendorDebugger
    {
        protected override IQsiLanguageService CreateLanguageService()
        {
            return new HanaLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new HanaRawParser();
        }
    }
}

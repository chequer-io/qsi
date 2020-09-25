using Qsi.Diagnostics;
using Qsi.JSql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.JSql
{
    internal sealed class JSqlDebugger : VendorDebugger
    {
        protected override IQsiLanguageService CreateLanguageService()
        {
            return new JSqlLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new JSqlRawParser();
        }
    }
}

using Qsi.Diagnostics;
using Qsi.JSql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.JSql
{
    internal sealed class JSqlDebugger : VendorDebugger
    {
        public override IQsiLanguageService CreateLanguageService()
        {
            return new JSqlLanguageService();
        }

        public override IRawTreeParser CreateRawTreeParser()
        {
            return new JSqlRawParser();
        }
    }
}

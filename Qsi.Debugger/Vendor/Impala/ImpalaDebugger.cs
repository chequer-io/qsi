using Qsi.Diagnostics;
using Qsi.Impala.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Impala
{
    internal class ImpalaDebugger : VendorDebugger
    {
        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new ImpalaRawParser();
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new ImpalaLanguageService();
        }
    }
}

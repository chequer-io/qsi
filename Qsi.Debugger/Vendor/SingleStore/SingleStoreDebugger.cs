using Qsi.Diagnostics;
using Qsi.Services;
using Qsi.SingleStore.Diagnostics;

namespace Qsi.Debugger.Vendor.SingleStore;

internal sealed class SingleStoreDebugger : VendorDebugger
{
    protected override IRawTreeParser CreateRawTreeParser()
    {
        return new SingleStoreRawParser();
    }

    protected override IQsiLanguageService CreateLanguageService()
    {
        return new SingleStoreLanguageService();
    }
}

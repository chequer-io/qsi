using Qsi.Services;
using Qsi.SingleStore;

namespace Qsi.Debugger.Vendor.SingleStore;

internal sealed class SingleStoreLanguageService : SingleStoreLanguageServiceBase
{
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new SingleStoreRepositoryProvider();
    }
}

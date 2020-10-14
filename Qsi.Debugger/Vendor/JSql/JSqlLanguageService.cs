using Qsi.JSql;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.JSql
{
    internal class JSqlLanguageService : JSqlLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new JSqlRepositoryProvider();
        }
    }
}

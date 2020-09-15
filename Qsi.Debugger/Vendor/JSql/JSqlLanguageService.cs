using Qsi.JSql;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.JSql
{
    internal class JSqlLanguageService : JSqlLanguageServiceBase
    {
        public override IQsiReferenceResolver CreateResolver()
        {
            return new JSqlReferenceResolver();
        }
    }
}

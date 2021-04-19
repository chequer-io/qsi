using Qsi.Hana;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Hana
{
    internal class HanaLanguageService : HanaLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new HanaRepositoryProvider();
        }
    }
}

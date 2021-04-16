using Qsi.Cql;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Hana
{
    internal class HanaLanguageService : CqlLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new HanaRepositoryProvider();
        }
    }
}

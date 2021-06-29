using Qsi.Impala;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Impala
{
    internal class ImpalaLanguageService : ImpalaLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new ImpalaRepositoryProvider();
        }
    }
}

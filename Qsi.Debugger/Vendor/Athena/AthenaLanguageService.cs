using Qsi.Debugger.Vendor.Trino;
using Qsi.Services;
using Qsi.Athena;

namespace Qsi.Debugger.Vendor.Athena;

internal class AthenaLanguageService : AthenaLanguageServiceBase
{
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new AthenaRepositoryProvider();
    }
}
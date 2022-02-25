using Qsi.Redshift;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Redshift;

public class RedshiftLanguageService : RedshiftLanguageServiceBase
{
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new RedshiftRepositoryProvider();
    }
}

using Qsi.Services;
using Qsi.Trino;

namespace Qsi.Debugger.Vendor.Trino;

public class TrinoLanguageService : TrinoLanguageServiceBase
{
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new TrinoRepositoryProvider();
    }
}
using Qsi.Oracle;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Oracle;

internal class OracleLanguageService : OracleLanguageServiceBase
{
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new OracleRepositoryProvider();
    }
}
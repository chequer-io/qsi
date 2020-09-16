using Qsi.Oracle;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Oracle
{
    internal sealed class OracleLanguageService : OracleLanguageServiceBase
    {
        public override IQsiReferenceResolver CreateResolver()
        {
            return new OracleReferenceResolver();
        }
    }
}

using Qsi.Cassandra;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Cql
{
    internal class CqlLanguageService : CqlLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new CqlRepositoryProvider();
        }
    }
}

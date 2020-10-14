using Qsi.MySql;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlLanguageService : MySqlLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new MySqlRepositoryProvider();
        }
    }
}

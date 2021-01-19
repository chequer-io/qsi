using System;
using Qsi.MySql;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlLanguageService : MySqlLanguageServiceBase
    {
        public override Version Version { get; }

        public MySqlLanguageService(Version version)
        {
            Version = version;
        }

        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new MySqlRepositoryProvider();
        }
    }
}

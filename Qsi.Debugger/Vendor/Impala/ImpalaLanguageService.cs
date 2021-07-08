using System;
using Qsi.Impala;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Impala
{
    internal class ImpalaLanguageService : ImpalaLanguageServiceBase
    {
        public override Version Version { get; }

        public ImpalaLanguageService(Version version)
        {
            Version = version;
        }

        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new ImpalaRepositoryProvider();
        }
    }
}

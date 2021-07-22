using System;
using Qsi.Impala;
using Qsi.Impala.Utilities;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Impala
{
    internal class ImpalaLanguageService : ImpalaLanguageServiceBase
    {
        public override ImpalaDialect Dialect { get; }

        public ImpalaLanguageService(Version version)
        {
            Dialect = ImpalaUtility.CreateDialect(version);
        }

        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new ImpalaRepositoryProvider();
        }
    }
}

using System;
using Qsi.Diagnostics;
using Qsi.Impala.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Impala
{
    internal class ImpalaDebugger : VendorDebugger
    {
        private readonly Version _verseion;

        public ImpalaDebugger(Version verseion)
        {
            _verseion = verseion;
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new ImpalaRawParser(_verseion);
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new ImpalaLanguageService(_verseion);
        }
    }
}

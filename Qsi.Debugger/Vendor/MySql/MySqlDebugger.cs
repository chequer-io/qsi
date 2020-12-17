using System;
using Qsi.Diagnostics;
using Qsi.MySql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlDebugger : VendorDebugger
    {
        private readonly Version _version;

        public MySqlDebugger(Version version)
        {
            _version = version;
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new MySqlLanguageService(_version);
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new MySqlRawParser(_version);
        }
    }
}

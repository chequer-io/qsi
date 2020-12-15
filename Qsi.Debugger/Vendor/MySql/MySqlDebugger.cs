using Qsi.Diagnostics;
using Qsi.MySql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlDebugger : VendorDebugger
    {
        private readonly int _version;

        public MySqlDebugger(int version)
        {
            _version = version;
        }

        protected override IQsiLanguageService CreateLanguageService()
        {
            return new MySqlLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new MySqlRawParser(_version);
        }
    }
}

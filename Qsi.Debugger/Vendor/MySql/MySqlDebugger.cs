using Qsi.Diagnostics;
using Qsi.MySql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlDebugger : VendorDebugger
    {
        public override IQsiLanguageService CreateLanguageService()
        {
            return new MySqlLanguageService();
        }

        public override IRawTreeParser CreateRawTreeParser()
        {
            return new MySqlRawParser();
        }
    }
}

using Qsi.Diagnostics;
using Qsi.MySql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlDebugger : VendorDebugger
    {
        protected override IQsiLanguageService CreateLanguageService()
        {
            return new MySqlLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new MySqlRawParser();
        }
    }
}

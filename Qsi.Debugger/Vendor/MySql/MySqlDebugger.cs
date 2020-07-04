using Qsi.MySql;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    class MySqlDebugger : VendorDebugger
    {
        public override IQsiTreeParser Parser { get; }

        public override IQsiLanguageService LanguageService { get; }

        public MySqlDebugger()
        {
            Parser = new MySqlParser();
            LanguageService = new MySqlLanguageService();
        }
    }
}

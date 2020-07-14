using Qsi.Diagnostics;
using Qsi.MySql;
using Qsi.MySql.Diagnostics;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MySql
{
    class MySqlDebugger : VendorDebugger
    {
        public override IQsiTreeParser Parser { get; }

        public override IRawTreeParser RawParser { get; }

        public override IQsiLanguageService LanguageService { get; }

        public MySqlDebugger()
        {
            Parser = new MySqlParser();
            RawParser = new MySqlRawParser();
            LanguageService = new MySqlLanguageService();
        }
    }
}

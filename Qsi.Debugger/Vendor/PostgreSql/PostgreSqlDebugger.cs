using Qsi.Diagnostics;
using Qsi.Parsing;
using Qsi.PostgreSql;
using Qsi.PostgreSql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PostgreSql
{
    internal class PostgreSqlDebugger : VendorDebugger
    {
        public override IQsiTreeParser Parser { get; }

        public override IRawTreeParser RawParser { get; }

        public override IQsiLanguageService LanguageService { get; }

        public PostgreSqlDebugger()
        {
            Parser = new PostgreSqlParser();
            RawParser = new PostgreSqlRawParser();
            LanguageService = new PostgreSqlLanguageService();
        }
    }
}

using Microsoft.SqlServer.Management.SqlParser.Common;
using Qsi.Diagnostics;
using Qsi.Parsing;
using Qsi.Services;
using Qsi.SqlServer.Diagnostics;

namespace Qsi.Debugger.Vendor.SqlServer
{
    internal class SqlServerDebugger : VendorDebugger
    {
        public override IQsiTreeParser Parser { get; }

        public override IRawTreeParser RawParser { get; }

        public override IQsiLanguageService LanguageService { get; }

        public SqlServerDebugger(DatabaseCompatibilityLevel compatibilityLevel)
        {
            RawParser = new SqlServerRawParser();
            LanguageService = new SqlServerLanguageService(compatibilityLevel);
            Parser = LanguageService.CreateTreeParser();
        }
    }
}

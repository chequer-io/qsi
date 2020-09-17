using Qsi.Diagnostics;
using Qsi.Parsing;
using Qsi.Services;
using Qsi.SqlServer;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Diagnostics;

namespace Qsi.Debugger.Vendor.SqlServer
{
    internal class SqlServerDebugger : VendorDebugger
    {
        public override IQsiTreeParser Parser { get; }

        public override IRawTreeParser RawParser { get; }

        public override IQsiLanguageService LanguageService { get; }

        public SqlServerDebugger(TransactSqlVersion transactSqlVersion)
        {
            RawParser = new SqlServerRawTreeParser(transactSqlVersion);
            LanguageService = new SqlServerLanguageService(transactSqlVersion);
            Parser = LanguageService.CreateTreeParser();
        }
    }
}

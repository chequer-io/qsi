using Qsi.Diagnostics;
using Qsi.Services;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Diagnostics;

namespace Qsi.Debugger.Vendor.SqlServer
{
    internal class SqlServerDebugger : VendorDebugger
    {
        private readonly TransactSqlVersion _transactSqlVersion;

        public SqlServerDebugger(TransactSqlVersion transactSqlVersion)
        {
            _transactSqlVersion = transactSqlVersion;
        }

        public override IQsiLanguageService CreateLanguageService()
        {
            return new SqlServerLanguageService(_transactSqlVersion);
        }

        public override IRawTreeParser CreateRawTreeParser()
        {
            return new SqlServerRawTreeParser(_transactSqlVersion);
        }
    }
}

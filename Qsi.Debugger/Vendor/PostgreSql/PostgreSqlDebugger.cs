using Qsi.Diagnostics;
using Qsi.PostgreSql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PostgreSql;

internal class PostgreSqlDebugger : VendorDebugger
{
    protected override IRawTreeParser CreateRawTreeParser()
    {
        return new PostgreSqlRawParser();
    }

    protected override IQsiLanguageService CreateLanguageService()
    {
        return new PostgreSqlLanguageService();
    }
}

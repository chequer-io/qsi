using Qsi.PhoenixSql.Diagnostics;
using Qsi.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PhoenixSql;

internal sealed class PhoenixSqlDebugger : VendorDebugger
{
    protected override IRawTreeParser CreateRawTreeParser()
    {
        return new PhoenixSqlRawParser();
    }

    protected override IQsiLanguageService CreateLanguageService()
    {
        return new PhoenixSqlLanguageService();
    }
}
using Qsi.Diagnostics;
using Qsi.PostgreSql.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Redshift;

internal class RedshiftDebugger : VendorDebugger
{
    protected override IRawTreeParser CreateRawTreeParser()
    {
        return new PostgreSqlLegacyRawParser();
    }

    protected override IQsiLanguageService CreateLanguageService()
    {
        return new RedshiftLanguageService();
    }
}

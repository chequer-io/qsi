using Qsi.Cassandra.Diagnostics;
using Qsi.Diagnostics;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Cql
{
    internal class CqlDebugger : VendorDebugger
    {
        protected override IQsiLanguageService CreateLanguageService()
        {
            return new CqlLanguageService();
        }

        protected override IRawTreeParser CreateRawTreeParser()
        {
            return new CqlRawParser();
        }
    }
}

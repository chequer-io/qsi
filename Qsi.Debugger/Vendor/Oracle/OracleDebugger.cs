using Qsi.Compiler;
using Qsi.Diagnostics;
using Qsi.JSql.Diagnostics;
using Qsi.Oracle.Compiler;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Oracle
{
    internal sealed class OracleDebugger : VendorDebugger
    {
        public override IQsiLanguageService CreateLanguageService()
        {
            return new OracleLanguageService();
        }

        public override IRawTreeParser CreateRawTreeParser()
        {
            return new JSqlRawParser();
        }

        public override QsiTableCompiler CreateCopmiler()
        {
            return new OracleTableCompiler(LanguageService);
        }
    }
}

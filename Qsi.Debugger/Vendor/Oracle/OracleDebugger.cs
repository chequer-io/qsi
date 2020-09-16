using Qsi.Compiler;
using Qsi.Diagnostics;
using Qsi.JSql.Diagnostics;
using Qsi.Oracle;
using Qsi.Oracle.Compiler;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.Oracle
{
    internal sealed class OracleDebugger : VendorDebugger
    {
        public override IQsiTreeParser Parser { get; }

        public override IRawTreeParser RawParser { get; }

        public override IQsiLanguageService LanguageService { get; }

        public OracleDebugger()
        {
            Parser = new OracleParser();
            RawParser = new JSqlRawParser();
            LanguageService = new OracleLanguageService();
        }

        public override QsiTableCompiler CreateCopmiler()
        {
            return new OracleTableCompiler(LanguageService);
        }
    }
}

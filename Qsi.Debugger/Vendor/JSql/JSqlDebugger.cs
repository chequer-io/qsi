using Qsi.Diagnostics;
using Qsi.JSql;
using Qsi.JSql.Diagnostics;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.JSql
{
    internal sealed class JSqlDebugger : VendorDebugger
    {
        public override IQsiTreeParser Parser { get; }

        public override IRawTreeParser RawParser { get; }

        public override IQsiLanguageService LanguageService { get; }

        public JSqlDebugger()
        {
            Parser = new JSqlParser();
            RawParser = new JSqlRawParser();
        }
    }
}

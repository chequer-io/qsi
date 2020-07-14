using Qsi.Diagnostics;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Debugger.Vendor
{
    internal abstract class VendorDebugger
    {
        public abstract IQsiTreeParser Parser { get; }

        public abstract IRawTreeParser RawParser { get; }

        public abstract IQsiLanguageService LanguageService { get; }
    }
}

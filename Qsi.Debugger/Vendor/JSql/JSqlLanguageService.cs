using Qsi.Data;
using Qsi.JSql;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.JSql
{
    internal class JSqlLanguageService : JSqlLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new JSqlRepositoryProvider();
        }

        public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            return VendorDebugger.HookFindParameter(parameters, node);
        }
    }
}

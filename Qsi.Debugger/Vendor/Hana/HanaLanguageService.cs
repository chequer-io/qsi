using Qsi.Data;
using Qsi.Hana;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.Hana
{
    internal class HanaLanguageService : HanaLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new HanaRepositoryProvider();
        }

        public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            return VendorDebugger.HookFindParameter(parameters, node);
        }
    }
}

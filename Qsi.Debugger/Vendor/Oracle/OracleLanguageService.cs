using Qsi.Data;
using Qsi.Oracle;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.Oracle
{
    internal sealed class OracleLanguageService : OracleLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new OracleRepositoryProvider();
        }

        public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            return VendorDebugger.HookFindParameter(parameters, node);
        }
    }
}

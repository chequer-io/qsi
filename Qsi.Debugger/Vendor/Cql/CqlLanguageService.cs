using Qsi.Cql;
using Qsi.Data;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.Cql;

internal class CqlLanguageService : CqlLanguageServiceBase
{
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new CqlRepositoryProvider();
    }

    public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
    {
        return VendorDebugger.HookFindParameter(parameters, node);
    }
}
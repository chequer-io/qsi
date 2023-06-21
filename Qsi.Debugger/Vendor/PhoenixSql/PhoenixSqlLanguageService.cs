using Qsi.Data;
using Qsi.PhoenixSql;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.PhoenixSql;

public class PhoenixSqlLanguageService : PhoenixSqlLanguageServiceBase
{
    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new PhoenixSqlRepositoryProvider();
    }

    public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
    {
        return VendorDebugger.HookFindParameter(parameters, node);
    }
}
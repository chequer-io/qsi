using Qsi.Data;
using Qsi.Services;
using Qsi.SqlServer;
using Qsi.SqlServer.Common;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.SqlServer;

public class SqlServerLanguageService : SqlServerLanguageServiceBase
{
    public SqlServerLanguageService(TransactSqlVersion transactSqlVersion) : base(transactSqlVersion)
    {
    }

    public override IQsiRepositoryProvider CreateRepositoryProvider()
    {
        return new SqlServerRepositoryProvider();
    }

    public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
    {
        return VendorDebugger.HookFindParameter(parameters, node);
    }
}
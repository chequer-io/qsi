using Qsi.Data;
using Qsi.PostgreSql;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.PostgreSql
{
    internal class PostgreSqlLegacyLanguageService : PostgreSqlLanguageLegacyServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new PostgreSqlRepositoryProvider();
        }

        public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            return VendorDebugger.HookFindParameter(parameters, node);
        }
    }
}

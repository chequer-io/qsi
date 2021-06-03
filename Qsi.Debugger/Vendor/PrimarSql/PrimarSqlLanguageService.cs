using Qsi.Data;
using Qsi.PrimarSql;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Debugger.Vendor.PrimarSql
{
    internal class PrimarSqlLanguageService : PrimarSqlLanguageServiceBase
    {
        public override IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new PrimarSqlRepositoryProvider();
        }

        public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            return VendorDebugger.HookFindParameter(parameters, node);
        }
    }
}

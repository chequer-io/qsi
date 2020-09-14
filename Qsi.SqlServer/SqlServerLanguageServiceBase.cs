using Qsi.Compiler;
using Qsi.Parsing;
using Qsi.Services;
using Qsi.SqlServer.Common;

namespace Qsi.SqlServer
{
    public abstract class SqlServerLanguageServiceBase : QsiLanguageServiceBase
    {
        private readonly TransactSqlVersion _transactSqlVersion;

        protected SqlServerLanguageServiceBase(TransactSqlVersion transactSqlVersion)
        {
            _transactSqlVersion = transactSqlVersion;
        }

        public override IQsiTreeParser CreateTreeParser()
        {
            return new SqlServerParser(_transactSqlVersion);
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new SqlServerScriptParser();
        }

        public override QsiTableCompileOptions CreateCompileOptions()
        {
            return new QsiTableCompileOptions
            {
                AllowEmptyColumnsInSelect = false
            };
        }
    }
}

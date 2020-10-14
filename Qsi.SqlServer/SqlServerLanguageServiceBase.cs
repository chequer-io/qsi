using System;
using Qsi.Analyzers;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Services;
using Qsi.SqlServer.Common;
using Qsi.Utilities;

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

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            throw new NotImplementedException();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new SqlServerScriptParser(_transactSqlVersion);
        }

        public override bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y)
        {
            string nX = x.IsEscaped ? IdentifierUtility.Unescape(x.Value) : x.Value;
            string nY = y.IsEscaped ? IdentifierUtility.Unescape(y.Value) : y.Value;

            return string.Equals(nX, nY, StringComparison.OrdinalIgnoreCase);
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new QsiAnalyzerOptions
            {
                AllowEmptyColumnsInSelect = false
            };
        }
    }
}

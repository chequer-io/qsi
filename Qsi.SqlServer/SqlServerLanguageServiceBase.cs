using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Definition;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Services;
using Qsi.SqlServer.Analyzers;
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
            return new SqlServerDeparser();
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

        public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new SqlServerActionAnalyzer(engine);
            yield return new QsiTableAnalyzer(engine);
            yield return new QsiDefinitionAnalyzer(engine);
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new()
            {
                AllowEmptyColumnsInSelect = false
            };
        }
    }
}

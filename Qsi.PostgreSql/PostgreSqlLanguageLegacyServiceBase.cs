using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.PostgreSql.Analyzers;
using Qsi.Services;

namespace Qsi.PostgreSql
{
    public abstract class PostgreSqlLanguageLegacyServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new PostgreSqlLegacyParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new PostgreSqlDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new PostgreSqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new()
            {
                AllowEmptyColumnsInSelect = true,
                AllowEmptyColumnsInInline = true,
                AllowNoAliasInDerivedTable = true
            };
        }

        public override IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new PgTableAnalyzer(engine);
            yield return new QsiDefinitionAnalyzer(engine);
        }
    }
}

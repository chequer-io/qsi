using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Definition;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.PrimarSql.Analyzers;
using Qsi.Services;

namespace Qsi.PrimarSql
{
    public abstract class PrimarSqlLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return PrimarSqlParser.Instance;
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new PrimarSqlDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new PrimarSqlScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new()
            {
                AllowEmptyColumnsInSelect = false
            };
        }

        public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new PrimarSqlTableAnalyzer(engine);
            yield return new PrimarSqlActionAnalyzer(engine);
            yield return new QsiDefinitionAnalyzer(engine);
        }
    }
}

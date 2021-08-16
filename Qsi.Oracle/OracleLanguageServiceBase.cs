using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Table;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Oracle
{
    public abstract class OracleLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new OracleParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new OracleDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new OracleScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new();
        }

        public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new QsiTableAnalyzer(engine);
        }
    }
}

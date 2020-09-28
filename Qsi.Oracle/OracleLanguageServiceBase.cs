using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.JSql;
using Qsi.Parsing;

namespace Qsi.Oracle
{
    public abstract class OracleLanguageServiceBase : JSqlLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new OracleParser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new OracleScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new QsiAnalyzerOptions
            {
                AllowNoAliasInDerivedTable = true,
                UseAutoFixRecursiveQuery = true
            };
        }

        public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new OracleTableAnalyzer(engine);
        }
    }
}

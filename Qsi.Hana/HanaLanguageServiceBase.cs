using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Hana.Analyzers;
using Qsi.Hana.Tree;
using Qsi.Parsing;
using Qsi.Parsing.Common;
using Qsi.Services;

namespace Qsi.Hana
{
    public abstract class HanaLanguageServiceBase : QsiLanguageServiceBase
    {
        public override IQsiTreeParser CreateTreeParser()
        {
            return new HanaParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new HanaDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new CommonScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new();
        }

        public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new HanaTableAnalyzer(engine);
        }
    }
}

using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Table;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Parsing.Common;
using Qsi.Services;
using Qsi.Trino.Analyzers;

namespace Qsi.Trino
{
    public abstract class TrinoLanguageServiceBase : QsiLanguageServiceBase
    {
        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new();
        }

        public override IQsiTreeParser CreateTreeParser()
        {
            return new TrinoParser();
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new TrinoDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new TrinoScriptParser();
        }

        public override IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
        {
            yield return new TrinoActionAnalyzer(engine);
            yield return new QsiTableAnalyzer(engine);
        }
    }
}

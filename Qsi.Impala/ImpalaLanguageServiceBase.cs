using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Engines;
using Qsi.Impala.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Impala
{
    public abstract class ImpalaLanguageServiceBase : QsiLanguageServiceBase
    {
        public abstract ImpalaDialect Dialect { get; }

        public override IQsiTreeParser CreateTreeParser()
        {
            return new ImpalaParser(Dialect);
        }

        public override IQsiTreeDeparser CreateTreeDeparser()
        {
            return new ImpalaDeparser();
        }

        public override IQsiScriptParser CreateScriptParser()
        {
            return new ImpalaScriptParser();
        }

        public override QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new();
        }

        public override IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new ImpalaTableAnalyzer(engine);
            yield return new QsiDefinitionAnalyzer(engine);
        }
    }
}

using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Definition;
using Qsi.Cql.Analyzers;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Cql;

public abstract class CqlLanguageServiceBase : QsiLanguageServiceBase
{
    public override IQsiTreeParser CreateTreeParser()
    {
        return CqlParser.Instance;
    }

    public override IQsiTreeDeparser CreateTreeDeparser()
    {
        return new CqlDeparser();
    }

    public override IQsiScriptParser CreateScriptParser()
    {
        return new CqlScriptParser();
    }

    public override QsiAnalyzerOptions CreateAnalyzerOptions()
    {
        return new();
    }

    public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
    {
        yield return new CqlTableAnalyzer(engine);
        yield return new CqlActionAnalyzer(engine);
        yield return new QsiDefinitionAnalyzer(engine);
    }
}
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.PhoenixSql.Analyzers;
using Qsi.Services;

namespace Qsi.PhoenixSql;

public abstract class PhoenixSqlLanguageServiceBase : QsiLanguageServiceBase
{
    public override IQsiTreeParser CreateTreeParser()
    {
        return new PhoenixSqlParser();
    }

    public override IQsiTreeDeparser CreateTreeDeparser()
    {
        return new PhoenixSqlDeparser();
    }

    public override IQsiScriptParser CreateScriptParser()
    {
        return new PhoenixSqlScriptParser();
    }

    public override QsiAnalyzerOptions CreateAnalyzerOptions()
    {
        return new();
    }
        
    public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
    {
        yield return new QsiActionAnalyzer(engine);
        yield return new PhoenixSqlTableAnalyzer(engine);
        yield return new QsiDefinitionAnalyzer(engine);
    }
}
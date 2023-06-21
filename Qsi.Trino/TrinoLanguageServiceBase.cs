using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Definition;
using Qsi.Collections;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Services;
using Qsi.Trino.Analyzers;

namespace Qsi.Trino;

public abstract class TrinoLanguageServiceBase : QsiLanguageServiceBase
{
    protected override IEqualityComparer<QsiIdentifier> GetIdentifierComparer()
    {
        return new QsiIdentifierEqualityComparer(StringComparison.OrdinalIgnoreCase);
    }

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
        yield return new TrinoTableAnalyzer(engine);
        yield return new QsiDefinitionAnalyzer(engine);
    }
}
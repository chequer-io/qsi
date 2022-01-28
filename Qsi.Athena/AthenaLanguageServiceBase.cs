using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Athena.Analyzers;
using Qsi.Collections;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Athena;

public abstract class AthenaLanguageServiceBase : QsiLanguageServiceBase
{
    protected override IEqualityComparer<QsiIdentifier> GetIdentifierComparer()
    {
        return new QsiIdentifierEqualityComparer(StringComparison.OrdinalIgnoreCase);
    }

    public override QsiAnalyzerOptions CreateAnalyzerOptions()
    {
        return new QsiAnalyzerOptions();
    }

    public override IQsiTreeParser CreateTreeParser()
    {
        return new AthenaParser();
    }

    public override IQsiScriptParser CreateScriptParser()
    {
        return new AthenaScriptParser();
    }

    public override IQsiTreeDeparser CreateTreeDeparser()
    {
        return new AthenaDeparser();
    }

    public override IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
    {
        yield return new AthenaActionAnalyzer(engine);
        yield return new AthenaTableAnalyzer(engine);
    }
}

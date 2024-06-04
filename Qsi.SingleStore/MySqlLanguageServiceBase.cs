using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Collections;
using Qsi.Data;
using Qsi.Engines;
using Qsi.SingleStore.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.SingleStore;

public abstract class SingleStoreLanguageServiceBase : QsiLanguageServiceBase
{
    public abstract Version Version { get; }

    public abstract bool MariaDBCompatibility { get; }

    protected override IEqualityComparer<QsiIdentifier> GetIdentifierComparer()
    {
        return new QsiIdentifierEqualityComparer(StringComparison.OrdinalIgnoreCase);
    }

    public override IQsiTreeParser CreateTreeParser()
    {
        return new SingleStoreParser();
    }

    public override IQsiTreeDeparser CreateTreeDeparser()
    {
        return new SingleStoreDeparser();
    }

    public override IQsiScriptParser CreateScriptParser()
    {
        return new SingleStoreScriptParser();
    }

    public override QsiAnalyzerOptions CreateAnalyzerOptions()
    {
        return new QsiAnalyzerOptions
        {
            AllowEmptyColumnsInSelect = false
        };
    }

    public override IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
    {
        yield return new SingleStoreActionAnalyzer(engine);
        yield return new SingleStoreTableAnalyzer(engine);
        yield return new QsiDefinitionAnalyzer(engine);
    }
}

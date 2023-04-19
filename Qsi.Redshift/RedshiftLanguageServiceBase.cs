using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Definition;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Redshift.Analyzers;
using Qsi.Services;

namespace Qsi.Redshift;

public abstract class RedshiftLanguageServiceBase : QsiLanguageServiceBase
{
    public int TotalStack { get; set; } = 1024 * 1024 * 25; // 25MB

    public ulong TotalMemory { get; set; } = 1024 * 1024 * 1024; // 1GB

    public override IQsiTreeParser CreateTreeParser()
    {
        return new RedshiftParser(TotalStack, TotalMemory);
    }

    public override IQsiTreeDeparser CreateTreeDeparser()
    {
        return new RedshiftDeparser();
    }

    public override IQsiScriptParser CreateScriptParser()
    {
        return new RedshiftScriptParser();
    }

    public override QsiAnalyzerOptions CreateAnalyzerOptions()
    {
        return new QsiAnalyzerOptions
        {
            AllowEmptyColumnsInSelect = true,
            AllowEmptyColumnsInInline = true,
            AllowNoAliasInDerivedTable = true
        };
    }

    public override IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
    {
        yield return new QsiActionAnalyzer(engine);
        yield return new RedshiftTableAnalyzer(engine);
        yield return new QsiDefinitionAnalyzer(engine);
    }
}

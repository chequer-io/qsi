using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Definition;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Hana.Analyzers;
using Qsi.Parsing;
using Qsi.Parsing.Common;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Hana;

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
        yield return new HanaActionAnalyzer(engine);
        yield return new HanaTableAnalyzer(engine);
        yield return new QsiDefinitionAnalyzer(engine);
    }

    public override QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
    {
        if (node.Type == QsiParameterType.Index && node.Index.HasValue)
            return parameters[node.Index.Value - 1];

        return base.FindParameter(parameters, node);
    }
}
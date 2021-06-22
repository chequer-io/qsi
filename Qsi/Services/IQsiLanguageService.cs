using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.Services
{
    public interface IQsiLanguageService
    {
        QsiAnalyzerOptions CreateAnalyzerOptions();

        IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine);

        IQsiTreeParser CreateTreeParser();

        IQsiTreeDeparser CreateTreeDeparser();

        IQsiScriptParser CreateScriptParser();

        IQsiRepositoryProvider CreateRepositoryProvider();

        bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y);

        QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node);
    }
}

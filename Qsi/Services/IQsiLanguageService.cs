using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.Services
{
    public interface IQsiLanguageService
    {
        QsiAnalyzerOptions CreateAnalyzerOptions();

        IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine);

        IQsiTreeParser CreateTreeParser();

        IQsiTreeDeparser CreateTreeDeparser();

        IQsiScriptParser CreateScriptParser();

        IQsiRepositoryProvider CreateRepositoryProvider();

        bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y);

        QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node);
    }
}

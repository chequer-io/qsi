using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Data;
using Qsi.Parsing;

namespace Qsi.Services
{
    public interface IQsiLanguageService
    {
        QsiAnalyzerOptions CreateAnalyzerOptions();

        IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine);

        IQsiTreeParser CreateTreeParser();

        IQsiScriptParser CreateScriptParser();

        IQsiReferenceResolver CreateReferenceResolver();

        bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y);
    }
}

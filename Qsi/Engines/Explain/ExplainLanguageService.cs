using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Services;
using Qsi.Tree;

namespace Qsi.Engines.Explain
{
    internal sealed class ExplainLanguageService : IQsiLanguageService
    {
        public QsiEngine ExplainEngine { get; set; }

        private readonly IQsiLanguageService _languageService;

        public ExplainLanguageService(IQsiLanguageService languageService)
        {
            _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        }

        public QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return _languageService.CreateAnalyzerOptions();
        }

        public IEnumerable<IQsiAnalyzer> CreateAnalyzers(QsiEngine engine)
        {
            foreach (var analyzer in _languageService.CreateAnalyzers(engine))
            {
                if (analyzer is QsiActionAnalyzer actionAnalyzer)
                    yield return new ExplainActionAnalyzer(actionAnalyzer);
                else
                    yield return analyzer;
            }
        }

        public IQsiTreeParser CreateTreeParser()
        {
            return _languageService.CreateTreeParser();
        }

        public IQsiTreeDeparser CreateTreeDeparser()
        {
            return _languageService.CreateTreeDeparser();
        }

        public IQsiScriptParser CreateScriptParser()
        {
            return _languageService.CreateScriptParser();
        }

        public IQsiRepositoryProvider CreateRepositoryProvider()
        {
            return new ExplainRepositoryProvider(ExplainEngine, _languageService.CreateRepositoryProvider());
        }

        public bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y)
        {
            return _languageService.MatchIdentifier(x, y);
        }

        public QsiParameter FindParameter(QsiParameter[] parameters, IQsiBindParameterExpressionNode node)
        {
            return _languageService.FindParameter(parameters, node);
        }
    }
}

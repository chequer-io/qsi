using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi
{
    public class QsiEngine
    {
        public IQsiTreeParser TreeParser => _treeParser.Value;

        public IQsiTreeDeparser TreeDeparser => _treeDeparser.Value;

        public IQsiScriptParser ScriptParser => _scriptParser.Value;

        public IQsiRepositoryProvider RepositoryProvider => _repositoryProvider.Value;

        public IQsiLanguageService LanguageService { get; }

        private readonly Lazy<IQsiTreeParser> _treeParser;
        private readonly Lazy<IQsiTreeDeparser> _treeDeparser;
        private readonly Lazy<IQsiScriptParser> _scriptParser;
        private readonly Lazy<IQsiRepositoryProvider> _repositoryProvider;
        private readonly Lazy<QsiAnalyzerBase[]> _analyzers;

        public QsiEngine(IQsiLanguageService languageService)
        {
            LanguageService = languageService;

            _treeParser = new Lazy<IQsiTreeParser>(LanguageService.CreateTreeParser);
            _treeDeparser = new Lazy<IQsiTreeDeparser>(LanguageService.CreateTreeDeparser);
            _scriptParser = new Lazy<IQsiScriptParser>(LanguageService.CreateScriptParser);
            _repositoryProvider = new Lazy<IQsiRepositoryProvider>(LanguageService.CreateRepositoryProvider);
            _analyzers = new Lazy<QsiAnalyzerBase[]>(() => LanguageService.CreateAnalyzers(this).ToArray());
        }

        public T GetAnalyzer<T>() where T : QsiAnalyzerBase
        {
            return _analyzers.Value.OfType<T>().First();
        }

        public async ValueTask<IQsiAnalysisResult[]> Execute(string input, CancellationToken cancellationToken = default)
        {
            var results = new List<IQsiAnalysisResult>();

            foreach (var script in ScriptParser.Parse(input, cancellationToken))
            {
                var result = await Execute(script, cancellationToken);

                if (result is EmptyAnalysisResult)
                    continue;

                results.Add(result);
            }

            return results.ToArray();
        }

        public async ValueTask<IQsiAnalysisResult> Execute(QsiScript script, CancellationToken cancellationToken = default)
        {
            var tree = TreeParser.Parse(script, cancellationToken);

            var options = LanguageService.CreateAnalyzerOptions();
            var analyzer = _analyzers.Value.FirstOrDefault(a => a.CanExecute(script, tree));

            if (analyzer == null)
            {
                if (script.ScriptType == QsiScriptType.Comment || script.ScriptType == QsiScriptType.Delimiter)
                    return new EmptyAnalysisResult();

                throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);
            }

            return await analyzer.Execute(script, tree, options, cancellationToken);
        }
    }
}

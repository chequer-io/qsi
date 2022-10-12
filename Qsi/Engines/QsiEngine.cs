using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Data;
using Qsi.Data.Cache;
using Qsi.Engines.Explain;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.Engines
{
    public class QsiEngine
    {
        public IQsiTreeParser TreeParser => _treeParser.Value;

        public IQsiTreeDeparser TreeDeparser => _treeDeparser.Value;

        public IQsiScriptParser ScriptParser => _scriptParser.Value;

        public IQsiRepositoryProvider RepositoryProvider => _repositoryProvider.Value;

        public Func<IQsiDataTableCacheProvider> CacheProviderFactory { get; }

        public IQsiLanguageService LanguageService { get; }

        internal bool IsExplainEngine => LanguageService is ExplainLanguageService;

        private readonly Lazy<IQsiTreeParser> _treeParser;
        private readonly Lazy<IQsiTreeDeparser> _treeDeparser;
        private readonly Lazy<IQsiScriptParser> _scriptParser;
        private readonly Lazy<IQsiRepositoryProvider> _repositoryProvider;
        private readonly Lazy<IQsiAnalyzer[]> _analyzers;

        public QsiEngine(IQsiLanguageService languageService) : this(languageService, () => new QsiDataTableMemoryCacheProvider())
        {
        }

        public QsiEngine(IQsiLanguageService languageService, Func<IQsiDataTableCacheProvider> cacheProviderFactory)
        {
            CacheProviderFactory = cacheProviderFactory;

            LanguageService = languageService;

            _treeParser = new Lazy<IQsiTreeParser>(LanguageService.CreateTreeParser);
            _treeDeparser = new Lazy<IQsiTreeDeparser>(LanguageService.CreateTreeDeparser);
            _scriptParser = new Lazy<IQsiScriptParser>(LanguageService.CreateScriptParser);
            _repositoryProvider = new Lazy<IQsiRepositoryProvider>(LanguageService.CreateRepositoryProvider);
            _analyzers = new Lazy<IQsiAnalyzer[]>(() => LanguageService.CreateAnalyzers(this).ToArray());
        }

        public T GetAnalyzer<T>() where T : QsiAnalyzerBase
        {
            return _analyzers.Value.OfType<T>().First();
        }

        public async ValueTask<IQsiAnalysisResult[]> Execute(QsiScript script, QsiParameter[] parameters, ExecuteOptions executeOptions = null, CancellationToken cancellationToken = default)
        {
            var tree = TreeParser.Parse(script, cancellationToken);

            var options = LanguageService.CreateAnalyzerOptions();
            var analyzer = _analyzers.Value.FirstOrDefault(a => a.CanExecute(script, tree));

            if (analyzer == null)
            {
                if (script.ScriptType is QsiScriptType.Trivia or QsiScriptType.Delimiter)
                    return Array.Empty<IQsiAnalysisResult>();

                throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);
            }

            return await analyzer.Execute(script, parameters, tree, options, executeOptions, cancellationToken);
        }

        public ValueTask<IQsiAnalysisResult[]> Explain(QsiScript script, ExecuteOptions executeOptions = null, CancellationToken cancellationToken = default)
        {
            var parameters = new[] { new QsiParameter(QsiParameterType.Name, string.Empty, QsiDataValue.Explain) };

            if (IsExplainEngine)
                return Execute(script, parameters, executeOptions, cancellationToken);

            var explainLanguageService = new ExplainLanguageService(LanguageService);
            var explainEngine = new QsiEngine(explainLanguageService);

            explainLanguageService.ExplainEngine = explainEngine;

            return explainEngine.Execute(script, parameters, executeOptions, cancellationToken);
        }
    }
}

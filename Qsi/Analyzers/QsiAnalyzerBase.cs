using System.Threading;
using System.Threading.Tasks;
using Qsi.Data;

namespace Qsi.Analyzers
{
    public abstract class QsiAnalyzerBase
    {
        public QsiEngine Engine { get; }

        protected QsiAnalyzerBase(QsiEngine engine)
        {
            Engine = engine;
        }

        public ValueTask<IQsiAnalysisResult> Execute(QsiScript script, QsiAnalyzerOptions options, CancellationToken cancellationToken = default)
        {
            if (!CanExecute(script, options))
                throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);

            return OnExecute(script, options, cancellationToken);
        }

        public abstract bool CanExecute(QsiScript script, QsiAnalyzerOptions options);

        protected abstract ValueTask<IQsiAnalysisResult> OnExecute(QsiScript script, QsiAnalyzerOptions options, CancellationToken cancellationToken = default);

        #region Utilities
        protected bool Match(QsiIdentifier a, QsiIdentifier b)
        {
            return Engine.LanguageService.MatchIdentifier(a, b);
        }

        protected bool Match(QsiQualifiedIdentifier a, QsiQualifiedIdentifier b)
        {
            if (a.Level != b.Level)
                return false;

            for (int i = 0; i < a.Level; i++)
            {
                if (!Match(a[i], b[i]))
                    return false;
            }

            return true;
        }
        #endregion
    }
}

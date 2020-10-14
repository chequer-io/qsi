using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Table;
using Qsi.Collections;
using Qsi.Data;
using Qsi.Parsing;

namespace Qsi.Services
{
    public abstract class QsiLanguageServiceBase : IQsiLanguageService
    {
        public abstract QsiAnalyzerOptions CreateAnalyzerOptions();

        public virtual IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new QsiTableAnalyzer(engine);
        }

        public abstract IQsiTreeParser CreateTreeParser();

        public abstract IQsiTreeDeparser CreateTreeDeparser();

        public abstract IQsiScriptParser CreateScriptParser();

        public abstract IQsiRepositoryProvider CreateRepositoryProvider();

        private IEqualityComparer<QsiIdentifier> _comparer;

        public virtual bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y)
        {
            _comparer ??= GetIdentifierComparer();
            return _comparer.Equals(x, y);
        }

        protected virtual IEqualityComparer<QsiIdentifier> GetIdentifierComparer()
        {
            return QsiIdentifierEqualityComparer.Default;
        }
    }
}
